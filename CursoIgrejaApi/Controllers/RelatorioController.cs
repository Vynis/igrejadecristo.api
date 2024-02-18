using CursoIgreja.Api.Helpers;
using CursoIgreja.Domain.Models;
using CursoIgreja.Domain.Models.Views;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        public RelatorioController(IVwContagemInscricaoCongregacaoRepository vwContagemInscricaoCongregacaoRepository, IVwContagemInscricaoCursoRepository vwContagemInscricaoCursoRepository, IVwRelatorioInscricoes vwRelatorioInscricoes )
        {
            _vwContagemInscricaoCongregacaoRepository = vwContagemInscricaoCongregacaoRepository;
            _vwContagemInscricaoCursoRepository = vwContagemInscricaoCursoRepository;
            _vwRelatorioInscricoes = vwRelatorioInscricoes;
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


    }
}
