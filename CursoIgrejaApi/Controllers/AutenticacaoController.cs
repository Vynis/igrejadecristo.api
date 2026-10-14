using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CursoIgreja.Api.Dtos;
using CursoIgreja.Api.Services;
using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IUsuariosRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IParametroSistemaRepository _parametroSistemaRepository;
        private readonly ILogUsuarioRepository _logUsuarioRepository;
        private readonly IRecuperacaoSenhaRepository _recuperacaoSenhaRepository;
        private readonly IResendEmailService _resendEmailService;
        private string urlEmailConfig = "";

        public AutenticacaoController(
            IUsuariosRepository usuariosRepository,
            IConfiguration configuration,
            IMapper mapper,
            IParametroSistemaRepository parametroSistemaRepository,
            ILogUsuarioRepository logUsuarioRepository,
            IRecuperacaoSenhaRepository recuperacaoSenhaRepository,
            IResendEmailService resendEmailService)
        {
            _usuarioRepository = usuariosRepository;
            _configuration = configuration;
            _mapper = mapper;
            _parametroSistemaRepository = parametroSistemaRepository;
            _logUsuarioRepository = logUsuarioRepository;
            _recuperacaoSenhaRepository = recuperacaoSenhaRepository;
            _resendEmailService = resendEmailService;

            var emailParam = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("Email")).Result?.FirstOrDefault();
            urlEmailConfig = emailParam?.Valor ?? "";
        }

        [HttpGet("gerar-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> GerarSenha(string senha)
        {
            try
            {
                return Response(SenhaHashService.CalculateMD5Hash(senha));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar(AutenticarDto autenticarDto)
        {
            try
            {
                autenticarDto.Senha = SenhaHashService.CalculateMD5Hash(autenticarDto.Senha);

                var response = await _usuarioRepository.Buscar(x => (x.Email.Equals(autenticarDto.Email) || x.Cpf.Equals(autenticarDto.Email)) && x.Senha.Equals(autenticarDto.Senha) && x.Status.Equals("A"));

                var usuario = _mapper.Map<UsuarioAutDto>(response.FirstOrDefault());

                if (usuario == null)
                    return Response("Usuário ou senha incorreto!", false);

                usuario.DadosComp = string.IsNullOrEmpty(response.Select(x => x.Rua).FirstOrDefault()) ? false : true;

                var geraLog = new GeraLogUsuario(_logUsuarioRepository, _usuarioRepository, usuario.Id).Gerar("Autenticar", "Logou no sistema").Result;

                var token = TokenService.GenerateToken(usuario, _configuration);

                return Response(new { usuario, token });

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        /// <summary>
        /// Solicita código de verificação de 6 dígitos via Resend (aceita DTO JSON)
        /// </summary>
        [HttpPost("solicitar-codigo-recuperacao")]
        [AllowAnonymous]
        public async Task<IActionResult> SolicitarCodigoRecuperacao([FromBody] SolicitarCodigoDto dto)
        {
            return await ProcessarSolicitacaoRecuperacao(dto?.EmailOuCpf);
        }

        /// <summary>
        /// Compatibilidade com chamadas existentes do app / web
        /// </summary>
        [HttpPost("recuperar-senha/{emailOuCpf}")]
        [AllowAnonymous]
        public async Task<IActionResult> RecuperarSenha(string emailOuCpf)
        {
            return await ProcessarSolicitacaoRecuperacao(emailOuCpf);
        }

        /// <summary>
        /// Valida o código numérico de 6 dígitos digitado pelo usuário e gera um token temporário
        /// </summary>
        [HttpPost("validar-codigo-recuperacao")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidarCodigoRecuperacao([FromBody] ValidarCodigoDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.EmailOuCpf) || string.IsNullOrWhiteSpace(dto.Codigo))
                    return Response("Informe o E-mail/CPF e o código de verificação.", false);

                var emailOuCpf = dto.EmailOuCpf.Trim();
                var codigo = dto.Codigo.Trim().Replace("-", "").Replace(" ", "");

                var usuarios = await _usuarioRepository.Buscar(x => (x.Email.Equals(emailOuCpf) || x.Cpf.Equals(emailOuCpf)) && x.Status.Equals("A"));
                var usuario = usuarios.FirstOrDefault();

                if (usuario == null)
                    return Response("Não encontramos nenhum cadastro com este E-mail ou CPF.", false);

                var registros = await _recuperacaoSenhaRepository.Buscar(x => x.UsuarioId == usuario.Id && x.Status.Equals("P") && x.DataExpiracao >= DateTime.Now);
                var registro = registros.OrderByDescending(x => x.DataCadastro).FirstOrDefault();

                if (registro == null)
                    return Response("Nenhum código ativo encontrado ou o prazo de 15 minutos expirou. Solicite um novo código.", false);

                if (registro.Tentativas >= 5)
                {
                    registro.Status = "C";
                    await _recuperacaoSenhaRepository.Atualizar(registro);
                    return Response("Número máximo de tentativas incorretas excedido. Solicite um novo código.", false);
                }

                if (!registro.Codigo.Equals(codigo))
                {
                    registro.Tentativas++;
                    await _recuperacaoSenhaRepository.Atualizar(registro);
                    var restantes = 5 - registro.Tentativas;
                    return Response($"Código de verificação incorreto. Você ainda tem {restantes} tentativa(s).", false);
                }

                // Código validado com sucesso! Gera token temporário
                registro.Status = "V"; // Validado
                registro.TokenTemporario = Guid.NewGuid().ToString("N");
                await _recuperacaoSenhaRepository.Atualizar(registro);

                return Response(new
                {
                    mensagem = "Código validado com sucesso!",
                    tokenTemporario = registro.TokenTemporario
                });
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        /// <summary>
        /// Redefine a senha do usuário usando o token temporário validado
        /// </summary>
        [HttpPost("redefinir-senha-com-codigo")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenhaComCodigo([FromBody] RedefinirSenhaCodigoDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.TokenTemporario) || string.IsNullOrWhiteSpace(dto.NovaSenha))
                    return Response("Dados incompletos para redefinição de senha.", false);

                var token = dto.TokenTemporario.Trim();

                // Busca token validado (status 'V') dentro da janela permitida
                var registros = await _recuperacaoSenhaRepository.Buscar(x => x.TokenTemporario.Equals(token) && x.Status.Equals("V") && x.DataExpiracao.AddMinutes(15) >= DateTime.Now);
                var registro = registros.FirstOrDefault();

                if (registro == null)
                    return Response("Sessão de redefinição expirada ou inválida. Por favor, reinicie o processo.", false);

                var usuario = await _usuarioRepository.ObterPorId(registro.UsuarioId);
                if (usuario == null)
                    return Response("Usuário não encontrado.", false);

                usuario.Senha = SenhaHashService.CalculateMD5Hash(dto.NovaSenha);
                var atualizou = await _usuarioRepository.Atualizar(usuario);

                if (!atualizou)
                    return Response("Não foi possível alterar a senha no momento. Tente novamente.", false);

                registro.Status = "U"; // Utilizado
                await _recuperacaoSenhaRepository.Atualizar(registro);

                var geraLog = new GeraLogUsuario(_logUsuarioRepository, _usuarioRepository, usuario.Id).Gerar("ResetarSenha", "Senha redefinida com sucesso via código").Result;

                return Response("Senha alterada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        /// <summary>
        /// Validação de link legado ou token temporário
        /// </summary>
        [HttpGet("valida-recuperacao-senha/{codigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidaRecuperacaoSenha(string codigo)
        {
            try
            {
                if (string.IsNullOrEmpty(codigo))
                    return Response("Favor preencher o codigo", false);

                // 1. Verifica se é um token temporário do novo fluxo (status 'V')
                var registros = await _recuperacaoSenhaRepository.Buscar(x => x.TokenTemporario.Equals(codigo) && x.Status.Equals("V") && x.DataExpiracao.AddMinutes(15) >= DateTime.Now);
                if (registros.Any())
                    return Response("Usuario encontrado");

                // 2. Fallback para decodificação legada (ID em Base64)
                var usuarioId = DecodificarCodigoLegado(codigo);
                if (usuarioId > 0)
                {
                    var buscaUsuario = await _usuarioRepository.ObterPorId(usuarioId);
                    if (buscaUsuario != null)
                        return Response("Usuario encontrado");
                }

                return Response("Usuario nao encontrado", false);
            }
            catch (Exception)
            {
                return Response("Usuario nao encontrado", false);
            }
        }

        /// <summary>
        /// Redefinição legada de senha via query param
        /// </summary>
        [HttpPost("resetar-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetarSenha(string codigo, string novaSenha)
        {
            try
            {
                if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(novaSenha))
                    return Response("Favor preencher o código e a nova senha", false);

                // 1. Verifica se é um token temporário do novo fluxo
                var registros = await _recuperacaoSenhaRepository.Buscar(x => x.TokenTemporario.Equals(codigo) && x.Status.Equals("V"));
                var registro = registros.FirstOrDefault();

                Usuarios usuarioAlvo = null;

                if (registro != null)
                {
                    usuarioAlvo = await _usuarioRepository.ObterPorId(registro.UsuarioId);
                    if (usuarioAlvo != null)
                    {
                        registro.Status = "U";
                        await _recuperacaoSenhaRepository.Atualizar(registro);
                    }
                }
                else
                {
                    // 2. Fallback para ID em Base64
                    var usuarioId = DecodificarCodigoLegado(codigo);
                    if (usuarioId > 0)
                        usuarioAlvo = await _usuarioRepository.ObterPorId(usuarioId);
                }

                if (usuarioAlvo == null)
                    return Response("Usuario nao encontrado", false);

                usuarioAlvo.Senha = SenhaHashService.CalculateMD5Hash(novaSenha);
                var response = await _usuarioRepository.Atualizar(usuarioAlvo);

                if (!response)
                    return Response("Não foi possível alterar a senha", false);

                var geraLog = new GeraLogUsuario(_logUsuarioRepository, _usuarioRepository, usuarioAlvo.Id).Gerar("ResetarSenha", "Alterou a senha").Result;

                return Response("Alteração realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        #region Métodos Auxiliares

        private async Task<IActionResult> ProcessarSolicitacaoRecuperacao(string emailOuCpf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailOuCpf))
                    return Response("Informe seu E-mail ou CPF.", false);

                var termo = emailOuCpf.Trim();
                var response = await _usuarioRepository.Buscar(x => (x.Email.Equals(termo) || x.Cpf.Equals(termo)) && x.Status.Equals("A"));
                var usuario = response.FirstOrDefault();

                if (usuario == null)
                    return Response("Não encontramos nenhum cadastro ativo com este E-mail ou CPF.", false);

                if (string.IsNullOrWhiteSpace(usuario.Email))
                    return Response("Este usuário não possui e-mail cadastrado. Favor entrar em contato com a equipe da igreja.", false);

                // Invalida códigos anteriores pendentes
                var codigosAnteriores = await _recuperacaoSenhaRepository.Buscar(x => x.UsuarioId == usuario.Id && x.Status.Equals("P"));
                foreach (var c in codigosAnteriores)
                {
                    c.Status = "C"; // Cancelado
                }
                if (codigosAnteriores.Any())
                {
                    await _recuperacaoSenhaRepository.AtualizarRange(codigosAnteriores);
                }

                // Gera código de 6 dígitos criptograficamente seguro
                var codigo = GerarCodigoNumerico(6);

                var recuperacao = new RecuperacaoSenha
                {
                    UsuarioId = usuario.Id,
                    Codigo = codigo,
                    DataExpiracao = DateTime.Now.AddMinutes(15),
                    Status = "P",
                    Tentativas = 0,
                    DataCadastro = DateTime.Now
                };

                await _recuperacaoSenhaRepository.Adicionar(recuperacao);

                // Envia e-mail via Resend
                var (sucessoEnvio, erroEnvio) = await _resendEmailService.EnviarCodigoRecuperacaoAsync(usuario.Email, usuario.Nome, codigo);

                if (!sucessoEnvio)
                {
                    return Response($"Erro ao enviar e-mail: {erroEnvio}", false);
                }

                var geraLog = new GeraLogUsuario(_logUsuarioRepository, _usuarioRepository, usuario.Id).Gerar("RecuperarSenha", "Solicitou código de recuperação de senha").Result;

                return Response(new
                {
                    mensagem = "Código de recuperação enviado com sucesso!",
                    emailMascarado = MascararEmail(usuario.Email),
                    possuiEmail = true
                });
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        private string GerarCodigoNumerico(int digitos = 6)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var num = Math.Abs(BitConverter.ToInt32(bytes, 0)) % (int)Math.Pow(10, digitos);
            return num.ToString($"D{digitos}");
        }

        private string MascararEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return "";

            var partes = email.Split('@');
            var usuario = partes[0];
            var dominio = partes[1];

            if (usuario.Length <= 2)
                return $"{usuario[0]}***@{dominio}";

            return $"{usuario.Substring(0, 2)}***{usuario.Substring(usuario.Length - 1)}@{dominio}";
        }

        private int DecodificarCodigoLegado(string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                    return 0;

                // Desfaz possíveis substituições de ModificaValor
                var limpo = codigo.Replace("\t", "")
                                  .Replace("%253D", "=")
                                  .Replace("%3D", "=")
                                  .Replace("%23", "#")
                                  .Replace("%25", "%")
                                  .Trim();

                // Completa padding se necessário
                var mod4 = limpo.Length % 4;
                if (mod4 > 0)
                    limpo += new string('=', 4 - mod4);

                var bytes = Convert.FromBase64String(limpo);
                var idStr = Encoding.ASCII.GetString(bytes);

                if (int.TryParse(idStr, out int id))
                    return id;
            }
            catch
            {
                // Ignora falhas de parse de Base64 legado
            }

            return 0;
        }

        public static string ModificaValor(string valor)
        {
            return valor?.Replace("=", "%3D").Replace("%", "%25").Replace("#", "%23") ?? "";
        }

        #endregion
    }
}
