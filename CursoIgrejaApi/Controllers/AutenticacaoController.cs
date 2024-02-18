using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        private string urlEmailConfig = "";

        public AutenticacaoController(IUsuariosRepository usuariosRepository, IConfiguration configuration, IMapper mapper, IParametroSistemaRepository parametroSistemaRepository, ILogUsuarioRepository logUsuarioRepository)
        {
            _usuarioRepository = usuariosRepository;
            _configuration = configuration;
            _mapper = mapper;
            _parametroSistemaRepository = parametroSistemaRepository;
            _logUsuarioRepository = logUsuarioRepository;
            urlEmailConfig = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("Email")).Result.FirstOrDefault().Valor;
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

                var response = await _usuarioRepository.Buscar(x =>( x.Email.Equals(autenticarDto.Email) || x.Cpf.Equals(autenticarDto.Email)) && x.Senha.Equals(autenticarDto.Senha) && x.Status.Equals("A"));

                var usuario = _mapper.Map<UsuarioAutDto>(response.FirstOrDefault());

                if (usuario == null)
                    return Response("Usuário ou senha incorreto!", false);

                usuario.DadosComp = string.IsNullOrEmpty(response.Select(x => x.Rua).FirstOrDefault()) ? false : true;

                var geraLog = new GeraLogUsuario(_logUsuarioRepository, _usuarioRepository, usuario.Id).Gerar("Autenticar", "Logou no sistema").Result;

                var token = TokenService.GenerateToken(usuario, _configuration);

                return Response(new { usuario , token });

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("recuperar-senha/{emailOuCpf}")]
        [AllowAnonymous]
        public async Task<IActionResult> RecuperarSenha(string emailOuCpf)
        {
            try
            {

                var response = await _usuarioRepository.Buscar(x => (x.Email.Equals(emailOuCpf) || x.Cpf.Equals(emailOuCpf)) && x.Status.Equals("A"));

                var usuario = response.FirstOrDefault();

                if (usuario == null)
                    return Response("Email ou senha não cadastrado no banco de dados!", false);

                var possuiEmail = true;

                if (string.IsNullOrEmpty(usuario.Email))
                {
                    possuiEmail = false;
                    usuario.Email = urlEmailConfig;
                }

                var linkAcesso = $"http://igrejadecristobrasil.com.br/app/reset/{ModificaValor(CriptografiaService.Criptografar(usuario.Id.ToString()))}";
                var titulo = "Summit Academy";
                var cabecalho = $"RECUPERAR SENHA - {usuario.Nome.ToUpper()}";
                var mensagem = $"Olá {usuario.Nome} <br /> <br /> <a href=\"{linkAcesso}\">Clique aqui</a> para renovar sua acesso. <br /> <br /> Att,<br /> Equipe Summit Academy ";

                var enviaEmail = new EnviaEmail(_parametroSistemaRepository);

                var retorno = await enviaEmail.Enviar(usuario.Email, cabecalho, mensagem, titulo);

                if (!retorno)
                    Response("Erro ao enviar email", false);

                var geraLog = new GeraLogUsuario(_logUsuarioRepository, _usuarioRepository, usuario.Id).Gerar("RecuperarSenha", "Solicitour recuperar a senha").Result;

                return Response( new { mensagem = "Envio com sucesso!", possuiEmail });
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("valida-recuperacao-senha/{codigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidaRecuperacaoSenha(string codigo)
        {
            try
            {
                if (string.IsNullOrEmpty(codigo))
                    return Response("Favor preencher o codigo", false);

                var usuarioId = CriptografiaService.Descriptografar(codigo);

                var buscaUsuario = await _usuarioRepository.ObterPorId(Convert.ToInt32(usuarioId));

                if (buscaUsuario == null)
                    return Response("Usuario nao encontrado", false);

                return Response("Usuario encontrado");

            }
            catch (Exception)
            {
                return Response("Usuario nao encontrado", false);
            }
        }
   
        [HttpPost("resetar-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetarSenha(string codigo, string novaSenha)
        {
            try
            {
                if (string.IsNullOrEmpty(codigo))
                    return Response("Favor preencher o codigo", false);

                var usuarioId = CriptografiaService.Descriptografar(codigo);

                var buscaUsuario = await _usuarioRepository.ObterPorId(Convert.ToInt32(usuarioId));

                if (buscaUsuario == null)
                    return Response("Usuario nao encontrado", false);

                buscaUsuario.Senha = SenhaHashService.CalculateMD5Hash(novaSenha);

                var response = await _usuarioRepository.Atualizar(buscaUsuario);

                if (!response)
                    return Response("Não foi possivel alterar a senha", false);

                var geraLog = new GeraLogUsuario(_logUsuarioRepository, _usuarioRepository, buscaUsuario.Id).Gerar("ResetarSenha", "Solicitour recuperar a senha").Result;

                return Response("Alteração realizada com sucesso.");

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }



        public static string ModificaValor(string valor)
        {
            return valor.Replace("=", "	%3D").Replace("%", "%25").Replace("#", "%23");
        }

    }
}
