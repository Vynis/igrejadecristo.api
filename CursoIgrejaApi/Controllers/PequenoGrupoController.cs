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
        private readonly IPequenoGrupoRelatorioPresencaRepository _pequenoGrupoRelatorioPresencaRepository;
        private readonly INotificacaoLiderPgRepository _notificacaoLiderPgRepository;
        private readonly INotificacaoLiderPgLeituraRepository _notificacaoLiderPgLeituraRepository;

        public PequenoGrupoController(
            IUsuariosRepository usuarioRepository,
            ILiderPequenoGrupoRepository liderPequenoGrupoRepository,
            IPequenoGrupoRepository pequenoGrupoRepository,
            IPequenoGrupoMembroRepository pequenoGrupoMembroRepository,
            IPequenoGrupoRelatorioRepository pequenoGrupoRelatorioRepository,
            IPequenoGrupoRelatorioPresencaRepository pequenoGrupoRelatorioPresencaRepository,
            INotificacaoLiderPgRepository notificacaoLiderPgRepository,
            INotificacaoLiderPgLeituraRepository notificacaoLiderPgLeituraRepository)
        {
            _usuarioRepository = usuarioRepository;
            _liderPequenoGrupoRepository = liderPequenoGrupoRepository;
            _pequenoGrupoRepository = pequenoGrupoRepository;
            _pequenoGrupoMembroRepository = pequenoGrupoMembroRepository;
            _pequenoGrupoRelatorioRepository = pequenoGrupoRelatorioRepository;
            _pequenoGrupoRelatorioPresencaRepository = pequenoGrupoRelatorioPresencaRepository;
            _notificacaoLiderPgRepository = notificacaoLiderPgRepository;
            _notificacaoLiderPgLeituraRepository = notificacaoLiderPgLeituraRepository;
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

        [HttpGet("admin/notificacoes")]
        public async Task<IActionResult> AdminBuscarNotificacoes()
        {
            try
            {
                var notificacoes = await _notificacaoLiderPgRepository.ObterTodos();

                return Response(notificacoes.OrderByDescending(x => x.DataCadastro).ToArray());
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("admin/notificacoes/{id}")]
        public async Task<IActionResult> AdminBuscarNotificacaoPorId(int id)
        {
            try
            {
                var notificacao = await _notificacaoLiderPgRepository.ObterPorId(id);

                if (notificacao == null)
                    return Response("Notificação não encontrada.", false);

                return Response(notificacao);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("admin/notificacoes")]
        public async Task<IActionResult> AdminCadastrarNotificacao(NotificacaoLiderPg notificacao)
        {
            try
            {
                if (string.IsNullOrEmpty(notificacao.Titulo) || string.IsNullOrEmpty(notificacao.Mensagem))
                    return Response("Título e mensagem são obrigatórios.", false);

                notificacao.Id = 0;
                notificacao.Status = string.IsNullOrEmpty(notificacao.Status) ? "A" : notificacao.Status;
                notificacao.DataCadastro = DateTime.Now;

                var response = await _notificacaoLiderPgRepository.Adicionar(notificacao);

                if (!response)
                    return Response("Erro ao cadastrar notificação.", false);

                return Response(notificacao);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("admin/notificacoes")]
        public async Task<IActionResult> AdminAtualizarNotificacao(NotificacaoLiderPg notificacao)
        {
            try
            {
                var notificacaoBanco = await _notificacaoLiderPgRepository.ObterPorId(notificacao.Id);

                if (notificacaoBanco == null)
                    return Response("Notificação não encontrada.", false);

                if (string.IsNullOrEmpty(notificacao.Titulo) || string.IsNullOrEmpty(notificacao.Mensagem))
                    return Response("Título e mensagem são obrigatórios.", false);

                notificacao.DataCadastro = notificacaoBanco.DataCadastro;
                notificacao.Status = string.IsNullOrEmpty(notificacao.Status) ? notificacaoBanco.Status : notificacao.Status;

                _notificacaoLiderPgRepository.DeatchLocal(x => x.Id == notificacao.Id);
                var response = await _notificacaoLiderPgRepository.Atualizar(notificacao);

                if (!response)
                    return Response("Erro ao atualizar notificação.", false);

                return Response("Atualização realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("admin/notificacoes/inativar/{id}")]
        public async Task<IActionResult> AdminInativarNotificacao(int id)
        {
            try
            {
                var notificacao = await _notificacaoLiderPgRepository.ObterPorId(id);

                if (notificacao == null)
                    return Response("Notificação não encontrada.", false);

                notificacao.Status = "I";
                var response = await _notificacaoLiderPgRepository.Atualizar(notificacao);

                if (!response)
                    return Response("Erro ao inativar notificação.", false);

                return Response("Notificação inativada com sucesso.");
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

                return Response(await MontarRelatoriosComPresencas(relatorios.OrderByDescending(x => x.DataReuniao).ToArray()));
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

                return Response(await MontarRelatorioComPresencas(relatorio));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("relatorios")]
        public async Task<IActionResult> CadastrarRelatorio(PequenoGrupoRelatorioRequest relatorio)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                var relatorioBanco = CriarRelatorio(relatorio, contexto);

                var response = await _pequenoGrupoRelatorioRepository.Adicionar(relatorioBanco);

                if (!response)
                    return Response("Erro ao cadastrar relatório.", false);

                await SalvarPresencas(relatorioBanco.Id, contexto.PequenoGrupo.Id, relatorio.Presencas);

                return Response(await MontarRelatorioComPresencas(relatorioBanco));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("relatorios")]
        public async Task<IActionResult> AtualizarRelatorio(PequenoGrupoRelatorioRequest relatorio)
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

                relatorioBanco.DataReuniao = relatorio.DataReuniao;
                relatorioBanco.SemanaReferencia = relatorio.SemanaReferencia;
                relatorioBanco.QuantidadeAtivos = relatorio.QuantidadeAtivos;
                relatorioBanco.QuantidadeRotativos = relatorio.QuantidadeRotativos;
                relatorioBanco.QuantidadeCriancas = relatorio.QuantidadeCriancas;
                relatorioBanco.QuantidadeVisitantes = relatorio.QuantidadeVisitantes;
                relatorioBanco.Observacao = relatorio.Observacao;
                relatorioBanco.Status = string.IsNullOrEmpty(relatorio.Status) ? relatorioBanco.Status : relatorio.Status;

                _pequenoGrupoRelatorioRepository.DeatchLocal(x => x.Id == relatorioBanco.Id);
                var response = await _pequenoGrupoRelatorioRepository.Atualizar(relatorioBanco);

                if (!response)
                    return Response("Erro ao atualizar relatório.", false);

                await SalvarPresencas(relatorioBanco.Id, contexto.PequenoGrupo.Id, relatorio.Presencas);

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

        [HttpGet("notificacoes")]
        public async Task<IActionResult> BuscarNotificacoes()
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                return Response(await MontarNotificacoes(contexto));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("notificacoes/resumo")]
        public async Task<IActionResult> BuscarResumoNotificacoes()
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                var notificacoes = await MontarNotificacoes(contexto);

                return Response(new
                {
                    quantidadeNaoLidas = notificacoes.Count(x => !x.Lida),
                    relatorioSemanalPendente = notificacoes.Any(x => x.Tipo == "RelatorioSemanal"),
                    notificacoes
                });
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("notificacoes/{id}/marcar-lida")]
        public async Task<IActionResult> MarcarNotificacaoLida(int id)
        {
            try
            {
                var contexto = await ObterContextoLider();

                if (!contexto.Permitido)
                    return Response(contexto.Mensagem, false);

                var notificacao = await _notificacaoLiderPgRepository.ObterPorId(id);

                if (notificacao == null)
                    return Response("Notificação não encontrada.", false);

                var leitura = (await _notificacaoLiderPgLeituraRepository.Buscar(x => x.NotificacaoId == id && x.LiderPequenoGrupoId == contexto.Lider.Id)).FirstOrDefault();

                if (leitura != null)
                    return Response("Notificação já marcada como lida.");

                var response = await _notificacaoLiderPgLeituraRepository.Adicionar(new NotificacaoLiderPgLeitura
                {
                    NotificacaoId = id,
                    LiderPequenoGrupoId = contexto.Lider.Id,
                    DataLeitura = DateTime.Now
                });

                if (!response)
                    return Response("Erro ao marcar notificação como lida.", false);

                return Response("Notificação marcada como lida.");
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

        private async Task<NotificacaoLiderPgResponse[]> MontarNotificacoes(ContextoLiderPequenoGrupo contexto)
        {
            var notificacoes = (await BuscarNotificacoesManuais(contexto)).ToList();

            if (await RelatorioSemanalPendente(contexto.PequenoGrupo.Id))
                notificacoes.Insert(0, CriarNotificacaoRelatorioSemanal());

            return notificacoes.OrderBy(x => x.Lida).ThenByDescending(x => x.Data).ToArray();
        }

        private async Task<NotificacaoLiderPgResponse[]> BuscarNotificacoesManuais(ContextoLiderPequenoGrupo contexto)
        {
            var hoje = DateTime.Now;
            var notificacoes = await _notificacaoLiderPgRepository.Buscar(x =>
                x.Status == "A" &&
                (x.DataInicio == null || x.DataInicio <= hoje) &&
                (x.DataFim == null || x.DataFim >= hoje) &&
                (x.CongregacaoId == null || x.CongregacaoId == contexto.PequenoGrupo.CongregacaoId) &&
                (x.PequenoGrupoId == null || x.PequenoGrupoId == contexto.PequenoGrupo.Id) &&
                (x.LiderPequenoGrupoId == null || x.LiderPequenoGrupoId == contexto.Lider.Id));

            var leituras = await _notificacaoLiderPgLeituraRepository.Buscar(x => x.LiderPequenoGrupoId == contexto.Lider.Id);
            var leiturasIds = leituras.Select(x => x.NotificacaoId).ToArray();

            return notificacoes.Select(x => new NotificacaoLiderPgResponse
            {
                Id = x.Id,
                Tipo = string.IsNullOrEmpty(x.Tipo) ? "Aviso" : x.Tipo,
                Titulo = x.Titulo,
                Mensagem = x.Mensagem,
                Data = x.DataInicio ?? x.DataCadastro,
                Lida = leiturasIds.Contains(x.Id),
                AcaoTexto = string.IsNullOrEmpty(x.Link) ? null : "Abrir",
                AcaoUrl = x.Link,
                Prioridade = "Normal"
            }).ToArray();
        }

        private async Task<bool> RelatorioSemanalPendente(int pequenoGrupoId)
        {
            var hoje = DateTime.Now;
            var inicioSemana = hoje.Date.AddDays(-(int)hoje.DayOfWeek);
            var fimSemana = inicioSemana.AddDays(7).AddTicks(-1);
            var relatorios = await _pequenoGrupoRelatorioRepository.Buscar(x =>
                x.PequenoGrupoId == pequenoGrupoId &&
                x.DataReuniao >= inicioSemana &&
                x.DataReuniao <= fimSemana &&
                (x.Status == "Rascunho" || x.Status == "Enviado"));

            return !relatorios.Any();
        }

        private NotificacaoLiderPgResponse CriarNotificacaoRelatorioSemanal()
        {
            return new NotificacaoLiderPgResponse
            {
                Id = 0,
                Tipo = "RelatorioSemanal",
                Titulo = "Relatório semanal pendente",
                Mensagem = $"Você ainda não preencheu o relatório da semana de {SemanaReferenciaAtual()}.",
                Data = DateTime.Now,
                Lida = false,
                AcaoTexto = "Preencher relatório",
                AcaoUrl = "/tablinks/relatorios",
                Prioridade = "Alta"
            };
        }

        private string SemanaReferenciaAtual()
        {
            var hoje = DateTime.Now;
            var inicioSemana = hoje.Date.AddDays(-(int)hoje.DayOfWeek);
            var fimSemana = inicioSemana.AddDays(6);
            return $"{inicioSemana:dd/MM} a {fimSemana:dd/MM}";
        }

        private PequenoGrupoRelatorio CriarRelatorio(PequenoGrupoRelatorioRequest request, ContextoLiderPequenoGrupo contexto)
        {
            return new PequenoGrupoRelatorio
            {
                Id = 0,
                PequenoGrupoId = contexto.PequenoGrupo.Id,
                LiderPequenoGrupoId = contexto.Lider.Id,
                DataReuniao = request.DataReuniao,
                SemanaReferencia = request.SemanaReferencia,
                QuantidadeAtivos = request.QuantidadeAtivos,
                QuantidadeRotativos = request.QuantidadeRotativos,
                QuantidadeCriancas = request.QuantidadeCriancas,
                QuantidadeVisitantes = request.QuantidadeVisitantes,
                Observacao = request.Observacao,
                Status = string.IsNullOrEmpty(request.Status) ? "Rascunho" : request.Status,
                DataCadastro = DateTime.Now
            };
        }

        private async Task SalvarPresencas(int relatorioId, int pequenoGrupoId, PequenoGrupoRelatorioPresencaRequest[] presencas)
        {
            var presencasBanco = await _pequenoGrupoRelatorioPresencaRepository.Buscar(x => x.PequenoGrupoRelatorioId == relatorioId);

            if (presencasBanco.Any())
                await _pequenoGrupoRelatorioPresencaRepository.RemoverRange(presencasBanco);

            if (presencas == null || !presencas.Any())
                return;

            var membros = await _pequenoGrupoMembroRepository.Buscar(x => x.PequenoGrupoId == pequenoGrupoId && x.Status == "A");
            var membrosIds = membros.Select(x => x.Id).ToArray();

            foreach (var presenca in presencas.Where(x => membrosIds.Contains(x.PequenoGrupoMembroId)))
            {
                await _pequenoGrupoRelatorioPresencaRepository.Adicionar(new PequenoGrupoRelatorioPresenca
                {
                    PequenoGrupoRelatorioId = relatorioId,
                    PequenoGrupoMembroId = presenca.PequenoGrupoMembroId,
                    Presente = presenca.Presente,
                    Observacao = presenca.Observacao
                });
            }
        }

        private async Task<object[]> MontarRelatoriosComPresencas(PequenoGrupoRelatorio[] relatorios)
        {
            var retorno = new object[relatorios.Length];

            for (var i = 0; i < relatorios.Length; i++)
                retorno[i] = await MontarRelatorioComPresencas(relatorios[i]);

            return retorno;
        }

        private async Task<object> MontarRelatorioComPresencas(PequenoGrupoRelatorio relatorio)
        {
            var presencas = await _pequenoGrupoRelatorioPresencaRepository.Buscar(x => x.PequenoGrupoRelatorioId == relatorio.Id);

            return new
            {
                relatorio.Id,
                relatorio.PequenoGrupoId,
                relatorio.LiderPequenoGrupoId,
                relatorio.DataReuniao,
                relatorio.SemanaReferencia,
                relatorio.QuantidadeAtivos,
                relatorio.QuantidadeRotativos,
                relatorio.QuantidadeCriancas,
                relatorio.QuantidadeVisitantes,
                relatorio.Observacao,
                relatorio.Status,
                relatorio.DataCadastro,
                relatorio.DataEnvio,
                Presencas = presencas.OrderBy(x => x.PequenoGrupoMembroId).ToArray()
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

        public class PequenoGrupoRelatorioRequest
        {
            public int Id { get; set; }
            public DateTime DataReuniao { get; set; }
            public string SemanaReferencia { get; set; }
            public int QuantidadeAtivos { get; set; }
            public int QuantidadeRotativos { get; set; }
            public int QuantidadeCriancas { get; set; }
            public int QuantidadeVisitantes { get; set; }
            public string Observacao { get; set; }
            public string Status { get; set; }
            public PequenoGrupoRelatorioPresencaRequest[] Presencas { get; set; }
        }

        public class PequenoGrupoRelatorioPresencaRequest
        {
            public int PequenoGrupoMembroId { get; set; }
            public bool Presente { get; set; }
            public string Observacao { get; set; }
        }

        public class NotificacaoLiderPgResponse
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
            public string Titulo { get; set; }
            public string Mensagem { get; set; }
            public DateTime Data { get; set; }
            public bool Lida { get; set; }
            public string AcaoTexto { get; set; }
            public string AcaoUrl { get; set; }
            public string Prioridade { get; set; }
        }
    }
}
