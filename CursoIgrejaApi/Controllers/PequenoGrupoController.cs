using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("pequeno-grupo")]
    [Authorize]
    public class PequenoGrupoController : ControllerBase
    {
        private readonly IUsuariosRepository _usuarioRepository;
        private readonly ILiderPequenoGrupoRepository _liderPequenoGrupoRepository;
        private readonly IPequenoGrupoRepository _pequenoGrupoRepository;
        private readonly IPequenoGrupoMembroRepository _pequenoGrupoMembroRepository;
        private readonly IPequenoGrupoRelatorioRepository _pequenoGrupoRelatorioRepository;

        public PequenoGrupoController(
            IUsuariosRepository usuarioRepository,
            ILiderPequenoGrupoRepository liderPequenoGrupoRepository,
            IPequenoGrupoRepository pequenoGrupoRepository,
            IPequenoGrupoMembroRepository pequenoGrupoMembroRepository,
            IPequenoGrupoRelatorioRepository pequenoGrupoRelatorioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _liderPequenoGrupoRepository = liderPequenoGrupoRepository;
            _pequenoGrupoRepository = pequenoGrupoRepository;
            _pequenoGrupoMembroRepository = pequenoGrupoMembroRepository;
            _pequenoGrupoRelatorioRepository = pequenoGrupoRelatorioRepository;
        }

        [HttpGet("meu-pg")]
        public async Task<IActionResult> MeuPg()
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                return Response(new
                {
                    lider = contexto.Lider,
                    pequenoGrupo = contexto.PequenoGrupo,
                    usuario = contexto.Usuario
                });
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("admin/lideres")]
        public async Task<IActionResult> AdminBuscarLideres()
        {
            try
            {
                var lideres = await _liderPequenoGrupoRepository.ObterTodos();

                var retorno = lideres
                    .OrderByDescending(x => x.Status)
                    .ThenBy(x => x.UsuarioId)
                    .ToArray();

                return Response(retorno);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("admin/lideres/{id}")]
        public async Task<IActionResult> AdminBuscarLiderPorId(int id)
        {
            try
            {
                var lider = await _liderPequenoGrupoRepository.ObterPorId(id);

                if (lider == null)
                    return Response("Líder não encontrado.", false);

                return Response(lider);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("admin/lideres")]
        public async Task<IActionResult> AdminCadastrarLider(LiderPequenoGrupo lider)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorId(lider.UsuarioId);

                if (usuario == null)
                    return Response("Usuário não encontrado.", false);

                var liderExistente = (await _liderPequenoGrupoRepository.Buscar(x => x.UsuarioId == lider.UsuarioId && x.Status == "A" && x.DataFim == null)).FirstOrDefault();

                if (liderExistente != null)
                    return Response("Usuário já está cadastrado como líder ativo de PG.", false);

                lider.Id = 0;
                lider.Status = string.IsNullOrEmpty(lider.Status) ? "A" : lider.Status;
                lider.DataInicio = lider.DataInicio == DateTime.MinValue ? DateTime.Now : lider.DataInicio;
                lider.DataCadastro = DateTime.Now;

                var response = await _liderPequenoGrupoRepository.Adicionar(lider);

                if (!response)
                    return Response("Erro ao cadastrar líder.", false);

                return Response(lider);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("admin/lideres")]
        public async Task<IActionResult> AdminAtualizarLider(LiderPequenoGrupo lider)
        {
            try
            {
                var liderBanco = await _liderPequenoGrupoRepository.ObterPorId(lider.Id);

                if (liderBanco == null)
                    return Response("Líder não encontrado.", false);

                var usuario = await _usuarioRepository.ObterPorId(lider.UsuarioId);

                if (usuario == null)
                    return Response("Usuário não encontrado.", false);

                lider.DataCadastro = liderBanco.DataCadastro;
                lider.Status = string.IsNullOrEmpty(lider.Status) ? liderBanco.Status : lider.Status;

                _liderPequenoGrupoRepository.DeatchLocal(x => x.Id == lider.Id);
                var response = await _liderPequenoGrupoRepository.Atualizar(lider);

                if (!response)
                    return Response("Erro ao atualizar líder.", false);

                return Response("Atualização realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("admin/lideres/inativar/{id}")]
        public async Task<IActionResult> AdminInativarLider(int id)
        {
            try
            {
                var lider = await _liderPequenoGrupoRepository.ObterPorId(id);

                if (lider == null)
                    return Response("Líder não encontrado.", false);

                lider.Status = "I";
                lider.DataFim = DateTime.Now;

                var response = await _liderPequenoGrupoRepository.Atualizar(lider);

                if (!response)
                    return Response("Erro ao inativar líder.", false);

                return Response("Líder inativado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("admin/pequenos-grupos")]
        public async Task<IActionResult> AdminBuscarPequenosGrupos()
        {
            try
            {
                var pequenosGrupos = await _pequenoGrupoRepository.ObterTodos();

                return Response(pequenosGrupos.OrderBy(x => x.Nome).ToArray());
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("admin/pequenos-grupos/{id}")]
        public async Task<IActionResult> AdminBuscarPequenoGrupoPorId(int id)
        {
            try
            {
                var pequenoGrupo = await _pequenoGrupoRepository.ObterPorId(id);

                if (pequenoGrupo == null)
                    return Response("PG não encontrado.", false);

                return Response(pequenoGrupo);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("admin/pequenos-grupos")]
        public async Task<IActionResult> AdminCadastrarPequenoGrupo(PequenoGrupo pequenoGrupo)
        {
            try
            {
                var lider = await _liderPequenoGrupoRepository.ObterPorId(pequenoGrupo.LiderPequenoGrupoId);

                if (lider == null || lider.Status != "A")
                    return Response("Líder ativo não encontrado.", false);

                pequenoGrupo.Id = 0;
                pequenoGrupo.Status = string.IsNullOrEmpty(pequenoGrupo.Status) ? "A" : pequenoGrupo.Status;
                pequenoGrupo.DataCadastro = DateTime.Now;

                var response = await _pequenoGrupoRepository.Adicionar(pequenoGrupo);

                if (!response)
                    return Response("Erro ao cadastrar PG.", false);

                return Response(pequenoGrupo);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("admin/pequenos-grupos")]
        public async Task<IActionResult> AdminAtualizarPequenoGrupo(PequenoGrupo pequenoGrupo)
        {
            try
            {
                var pequenoGrupoBanco = await _pequenoGrupoRepository.ObterPorId(pequenoGrupo.Id);

                if (pequenoGrupoBanco == null)
                    return Response("PG não encontrado.", false);

                var lider = await _liderPequenoGrupoRepository.ObterPorId(pequenoGrupo.LiderPequenoGrupoId);

                if (lider == null || lider.Status != "A")
                    return Response("Líder ativo não encontrado.", false);

                pequenoGrupo.DataCadastro = pequenoGrupoBanco.DataCadastro;
                pequenoGrupo.Status = string.IsNullOrEmpty(pequenoGrupo.Status) ? pequenoGrupoBanco.Status : pequenoGrupo.Status;

                _pequenoGrupoRepository.DeatchLocal(x => x.Id == pequenoGrupo.Id);
                var response = await _pequenoGrupoRepository.Atualizar(pequenoGrupo);

                if (!response)
                    return Response("Erro ao atualizar PG.", false);

                return Response("Atualização realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("membros")]
        public async Task<IActionResult> BuscarMembros()
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                var membros = await _pequenoGrupoMembroRepository.Buscar(x => x.PequenoGrupoId == contexto.PequenoGrupo.Id);

                return Response(membros.OrderBy(x => x.Nome).ToArray());
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("membros/{id}")]
        public async Task<IActionResult> BuscarMembroPorId(int id)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                var membro = (await _pequenoGrupoMembroRepository.Buscar(x => x.Id == id && x.PequenoGrupoId == contexto.PequenoGrupo.Id)).FirstOrDefault();

                if (membro == null)
                    return Response("Membro não encontrado.", false);

                return Response(membro);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("membros")]
        public async Task<IActionResult> CadastrarMembro(PequenoGrupoMembro membro)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                membro.Id = 0;
                membro.PequenoGrupoId = contexto.PequenoGrupo.Id;
                membro.Status = string.IsNullOrEmpty(membro.Status) ? "A" : membro.Status;
                membro.DataCadastro = DateTime.Now;

                var response = await _pequenoGrupoMembroRepository.Adicionar(membro);

                if (!response)
                    return Response("Erro ao cadastrar membro.", false);

                return Response(membro);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("membros")]
        public async Task<IActionResult> AtualizarMembro(PequenoGrupoMembro membro)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                var membroBanco = (await _pequenoGrupoMembroRepository.Buscar(x => x.Id == membro.Id && x.PequenoGrupoId == contexto.PequenoGrupo.Id)).FirstOrDefault();

                if (membroBanco == null)
                    return Response("Membro não encontrado.", false);

                membro.PequenoGrupoId = contexto.PequenoGrupo.Id;
                membro.DataCadastro = membroBanco.DataCadastro;
                membro.Status = string.IsNullOrEmpty(membro.Status) ? membroBanco.Status : membro.Status;

                _pequenoGrupoMembroRepository.DeatchLocal(x => x.Id == membro.Id);
                var response = await _pequenoGrupoMembroRepository.Atualizar(membro);

                if (!response)
                    return Response("Erro ao atualizar membro.", false);

                return Response("Atualização realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("membros/inativar/{id}")]
        public async Task<IActionResult> InativarMembro(int id)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);
                var membro = (await _pequenoGrupoMembroRepository.Buscar(x => x.Id == id && x.PequenoGrupoId == contexto.PequenoGrupo.Id)).FirstOrDefault();

                if (membro == null)
                    return Response("Membro não encontrado.", false);

                membro.Status = "I";
                membro.DataSaida = DateTime.Now;
                var response = await _pequenoGrupoMembroRepository.Atualizar(membro);

                if (!response)
                    return Response("Erro ao inativar membro.", false);

                return Response("Membro inativado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("relatorios")]
        public async Task<IActionResult> BuscarRelatorios()
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                var relatorios = await _pequenoGrupoRelatorioRepository.Buscar(x => x.PequenoGrupoId == contexto.PequenoGrupo.Id);

                return Response(relatorios.OrderByDescending(x => x.DataReuniao).ToArray());
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("relatorios/{id}")]
        public async Task<IActionResult> BuscarRelatorioPorId(int id)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);
                var relatorio = (await _pequenoGrupoRelatorioRepository.Buscar(x => x.Id == id && x.PequenoGrupoId == contexto.PequenoGrupo.Id)).FirstOrDefault();

                if (relatorio == null)
                    return Response("Relatório não encontrado.", false);

                return Response(relatorio);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("relatorios")]
        public async Task<IActionResult> CadastrarRelatorio(PequenoGrupoRelatorio relatorio)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                relatorio.Id = 0;
                relatorio.PequenoGrupoId = contexto.PequenoGrupo.Id;
                relatorio.LiderPequenoGrupoId = contexto.Lider.Id;
                relatorio.Status = string.IsNullOrEmpty(relatorio.Status) ? "Rascunho" : relatorio.Status;
                relatorio.DataCadastro = DateTime.Now;

                var response = await _pequenoGrupoRelatorioRepository.Adicionar(relatorio);

                if (!response)
                    return Response("Erro ao cadastrar relatório.", false);

                return Response(relatorio);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("relatorios")]
        public async Task<IActionResult> AtualizarRelatorio(PequenoGrupoRelatorio relatorio)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);
                var relatorioBanco = (await _pequenoGrupoRelatorioRepository.Buscar(x => x.Id == relatorio.Id && x.PequenoGrupoId == contexto.PequenoGrupo.Id)).FirstOrDefault();

                if (relatorioBanco == null)
                    return Response("Relatório não encontrado.", false);

                if (relatorioBanco.Status == "Enviado")
                    return Response("Relatório enviado não pode ser alterado.", false);

                relatorio.PequenoGrupoId = contexto.PequenoGrupo.Id;
                relatorio.LiderPequenoGrupoId = contexto.Lider.Id;
                relatorio.DataCadastro = relatorioBanco.DataCadastro;
                relatorio.Status = string.IsNullOrEmpty(relatorio.Status) ? relatorioBanco.Status : relatorio.Status;

                _pequenoGrupoRelatorioRepository.DeatchLocal(x => x.Id == relatorio.Id);
                var response = await _pequenoGrupoRelatorioRepository.Atualizar(relatorio);

                if (!response)
                    return Response("Erro ao atualizar relatório.", false);

                return Response("Atualização realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("relatorios/{id}/enviar")]
        public async Task<IActionResult> EnviarRelatorio(int id)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);
                var relatorio = (await _pequenoGrupoRelatorioRepository.Buscar(x => x.Id == id && x.PequenoGrupoId == contexto.PequenoGrupo.Id)).FirstOrDefault();

                if (relatorio == null)
                    return Response("Relatório não encontrado.", false);

                relatorio.Status = "Enviado";
                relatorio.DataEnvio = DateTime.Now;
                var response = await _pequenoGrupoRelatorioRepository.Atualizar(relatorio);

                if (!response)
                    return Response("Erro ao enviar relatório.", false);

                return Response("Relatório enviado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        private async Task<ContextoLiderPequenoGrupo> ObterContextoLider()
        {
            if (User?.Identity?.Name == null)
                return ContextoLiderPequenoGrupo.NaoPermitido("Usuário não autenticado.");

            var usuarioId = Convert.ToInt32(User.Identity.Name);
            var usuario = await _usuarioRepository.ObterPorId(usuarioId);

            if (usuario == null || usuario.Status != "A")
                return ContextoLiderPequenoGrupo.NaoPermitido("Usuário inativo ou não encontrado.");

            var lider = (await _liderPequenoGrupoRepository.Buscar(x => x.UsuarioId == usuarioId && x.Status == "A" && x.DataFim == null)).FirstOrDefault();

            if (lider == null)
                return ContextoLiderPequenoGrupo.NaoPermitido("Usuário sem permissão para acessar o módulo de PG.");

            var pequenoGrupo = (await _pequenoGrupoRepository.Buscar(x => x.Status == "A" && (x.LiderPequenoGrupoId == lider.Id || x.CoLiderPequenoGrupoId == lider.Id))).FirstOrDefault();

            if (pequenoGrupo == null)
                return ContextoLiderPequenoGrupo.NaoPermitido("Líder sem PG ativo vinculado.");

            return new ContextoLiderPequenoGrupo
            {
                Permitido = true,
                Usuario = usuario,
                Lider = lider,
                PequenoGrupo = pequenoGrupo
            };
        }

        private class ContextoLiderPequenoGrupo
        {
            public bool Permitido { get; set; }
            public string Mensagem { get; set; }
            public Usuarios Usuario { get; set; }
            public LiderPequenoGrupo Lider { get; set; }
            public PequenoGrupo PequenoGrupo { get; set; }

            public static ContextoLiderPequenoGrupo NaoPermitido(string mensagem)
            {
                return new ContextoLiderPequenoGrupo
                {
                    Permitido = false,
                    Mensagem = mensagem
                };
            }
        }
    }
}
