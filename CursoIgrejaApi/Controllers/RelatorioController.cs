using CursoIgreja.Api.Helpers;
using CursoIgreja.Domain.Models;
using CursoIgreja.Domain.Models.Views;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("relatorio")]
    public class RelatorioController : ControllerBase
    {
        private readonly IVwContagemInscricaoCongregacaoRepository _vwContagemInscricaoCongregacaoRepository;
        private readonly IVwContagemInscricaoCursoRepository _vwContagemInscricaoCursoRepository;
        private readonly IVwRelatorioInscricoes _vwRelatorioInscricoes;
        private readonly IRelatorioGeraisRepository _relatorioGeraisRepository;
        private readonly IInscricaoUsuarioRepository _inscricaoUsuarioRepository;

        public RelatorioController(IVwContagemInscricaoCongregacaoRepository vwContagemInscricaoCongregacaoRepository, IVwContagemInscricaoCursoRepository vwContagemInscricaoCursoRepository, IVwRelatorioInscricoes vwRelatorioInscricoes, IRelatorioGeraisRepository relatorioGeraisRepository, IInscricaoUsuarioRepository inscricaoUsuarioRepository )
        {
            _vwContagemInscricaoCongregacaoRepository = vwContagemInscricaoCongregacaoRepository;
            _vwContagemInscricaoCursoRepository = vwContagemInscricaoCursoRepository;
            _vwRelatorioInscricoes = vwRelatorioInscricoes;
            _relatorioGeraisRepository = relatorioGeraisRepository;
            _inscricaoUsuarioRepository = inscricaoUsuarioRepository;
        }




        [HttpPost("busca-contagem-inscricao-congregacao")]
        [AllowAnonymous]
        public async Task<IActionResult> BuscaContagemInscricaoCongregacao([FromBody] PaginationFilter filtro)
        {
            try
            {
                if (filtro.Filtro.Count() == 0)
                    return Response(await _vwContagemInscricaoCongregacaoRepository.ObterTodos());

                return Response(await _vwContagemInscricaoCongregacaoRepository.BuscaFiltroDinamico(filtro));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }


        [HttpPost("busca-contagem-inscricao-curso")]
        [AllowAnonymous]
        public async Task<IActionResult> BuscaContagemInscricaoCurso([FromBody] PaginationFilter filtro)
        {
            try
            {
                if (filtro.Filtro.Count() == 0)
                    return Response(await _vwContagemInscricaoCursoRepository.ObterTodos());

                return Response(await _vwContagemInscricaoCursoRepository.BuscaFiltroDinamico(filtro));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }


        [HttpGet("busca-todos-ciclos-com-inscricoes")]
        public async Task<IActionResult> BuscaTodosCiclosComInscricoes()
        {
            try
            {
                var result = await _vwRelatorioInscricoes.ObterTodos();

                var listaCiclos = result.Select(x => new { x.Ciclo }).Distinct().OrderBy(x => x.Ciclo).ToList();
                var listaAnos = result.Select(x => new { x.Ano }).Distinct().OrderByDescending(x => x.Ano).ToList();


                return Response(new { listaCiclos, listaAnos });
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("busca-relatorio-inscritos")]
        public async Task<IActionResult> BuscaTodosCiclosComInscricoes([FromBody] PaginationFilter filtro)
        {
            try
            {
                if (filtro.Filtro.Count() == 0)
                    return Response(await _vwRelatorioInscricoes.ObterTodos());

                return Response(await _vwRelatorioInscricoes.BuscaFiltroDinamico(filtro));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("download-relatorio-inscricoes")]
        public async Task<FileResult> DownloadRelatorioInscricoes([FromBody] PaginationFilter filtro)
        {
            try
            {
                var relatorio = new List<VwRelatorioInscricoes>();

                if (filtro.Filtro.Count() == 0)
                    relatorio = (await _vwRelatorioInscricoes.ObterTodos()).ToList();
                else
                    relatorio = (await _vwRelatorioInscricoes.BuscaFiltroDinamico(filtro)).ToList();

                var file = ExcelHelper.CreateFile(relatorio);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"inscricoes.xlsx");
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("relatorio-presenca-alunos/{ciclo}/{ano}/{processoInscricao}")]
        public async Task<IActionResult> RelatorioPresencaAlunos(int ciclo, int ano, int processoInscricao)
        {
            try
            {
                return Response(await _relatorioGeraisRepository.ObterTodos(ciclo, ano, processoInscricao));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }


        [HttpPost("download-relatorio-presenca-alunos")]
        public async Task<FileResult> DownloadRelatorioPresencaAlunos(int ciclo,  int ano,  int processoInscricao)
        {
            try
            {
                var relatorio = await _relatorioGeraisRepository.ObterTodos(ciclo, ano, processoInscricao);
                var file = ExcelHelper.CreateFile(relatorio);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"relatorio_presenca_alunos.xlsx");
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("relatorio-evolucao-usuarios")]
        public async Task<IActionResult> RelatorioEvolucaoUsuarios([FromQuery] string nome)
        {
            try
            {
                return Response(await ObterRelatorioEvolucaoUsuarios(nome));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("download-relatorio-evolucao-usuarios")]
        public async Task<FileResult> DownloadRelatorioEvolucaoUsuarios([FromQuery] string nome)
        {
            try
            {
                var relatorio = await ObterRelatorioEvolucaoUsuarios(nome);
                var file = ExcelHelper.CreateFile(relatorio);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"evolucao_alunos.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<dynamic>> ObterRelatorioEvolucaoUsuarios(string nome)
        {
            var inscricoesConfirmadas = (await _inscricaoUsuarioRepository.Buscar(x => x.Status.Equals("CO")))
                .Where(x => x.Usuario != null && x.ProcessoInscricao?.Curso != null && x.ProcessoInscricao.Curso.Status == "A")
                .ToList();

            if (!string.IsNullOrEmpty(nome))
                inscricoesConfirmadas = inscricoesConfirmadas
                    .Where(x => !string.IsNullOrEmpty(x.Usuario.Nome) && x.Usuario.Nome.ToLower().Contains(nome.ToLower()))
                    .ToList();

            var cursos = inscricoesConfirmadas
                .Select(x => x.ProcessoInscricao.Curso.Titulo)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var usuarios = inscricoesConfirmadas
                .GroupBy(x => new
                {
                    x.UsuarioId,
                    x.Usuario.Nome,
                    x.Usuario.TelefoneCelular
                })
                .OrderBy(x => x.Key.Nome)
                .ToList();

            var relatorio = new List<dynamic>();

            foreach (var usuario in usuarios)
            {
                dynamic linha = new ExpandoObject();
                var campos = (IDictionary<string, object>)linha;

                campos.Add("IdUsuario", usuario.Key.UsuarioId);
                campos.Add("Nome", usuario.Key.Nome);
                campos.Add("TelefoneCelular", usuario.Key.TelefoneCelular);

                foreach (var curso in cursos)
                    campos.Add(curso, "");

                foreach (var inscricao in usuario.Where(x => x.StatusEstudo == "AP"))
                    campos[inscricao.ProcessoInscricao.Curso.Titulo] = "X";

                relatorio.Add(linha);
            }

            return relatorio;
        }


    }
}
