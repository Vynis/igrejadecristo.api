using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("processo-inscricao")]
    public class ProcessoInscricaoController : ControllerBase
    {
        private readonly IProcessoInscricaoRepository _processoInscricaoRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IMapper _mapper;
        private readonly IInscricaoUsuarioRepository _inscricaoUsuarioRepository;

        public ProcessoInscricaoController(IProcessoInscricaoRepository processoInscricaoRepository, ICursoRepository cursoRepository, IMapper mapper, IInscricaoUsuarioRepository inscricaoUsuarioRepository)
        {
            _processoInscricaoRepository = processoInscricaoRepository;
            _cursoRepository = cursoRepository;
            _mapper = mapper;
            _inscricaoUsuarioRepository = inscricaoUsuarioRepository;
        }


        [HttpGet("cursos-inscricoes-abertas")]
        [AllowAnonymous]
        public async Task<IActionResult> CursosInscricoesAbertas()
        {
            try
            {
                var listaBd = await _processoInscricaoRepository.Buscar(x => x.Status.Equals("A") && DateTime.Now >= x.DataInicial && DateTime.Now <= x.DataFinal);

                return Response(listaBd);

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("cursos-inscricoes-futuras")]
        public async Task<IActionResult> CursosInscricoesFuturas()
        {
            try
            {
                var listaBd = await _processoInscricaoRepository.Buscar(x => x.Status.Equals("A") && x.DataFinal > DateTime.Now &&  x.DataFinal > DateTime.Now );

                return Response(listaBd);

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("cursos-inscricoes-disponivel")]
        public async Task<IActionResult> CursosInscricoesDisponivel()
        {
            try
            {
                var listaBd = await _processoInscricaoRepository.Buscar(x => x.Status.Equals("A") && DateTime.Now >= x.DataInicial && DateTime.Now <= x.DataFinal);

                var listaUsuario = await _inscricaoUsuarioRepository.Buscar(x => x.UsuarioId == Convert.ToInt32(User.Identity.Name));

                var listaNova = new List<ProcessoInscricao>();

                foreach (var lista in listaBd)
                {
                    var validaUsuario = listaUsuario.Where(x => x.ProcessoInscricaoId.Equals(lista.Id) && !x.Status.Equals("CA"));

                    if (validaUsuario.Count() == 0)
                        listaNova.Add(lista);
                }

                return Response(listaNova);

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("busca-inscricoes-ativos")]
        public async Task<IActionResult> BuscaProcessoInscricaoAtivos()
        {
            try
            {
                var listaBd = await _processoInscricaoRepository.Buscar(x => x.Status.Equals("A"));

                return Response(listaBd.OrderByDescending(x => x.Ano).ThenByDescending(x => x.Ciclo));

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }


        [HttpGet("buscar-ciclos")]
        [AllowAnonymous]
        public async Task<IActionResult> BuscarCiclos()
        {
            try
            {
                var listaBd = await _processoInscricaoRepository.ObterTodos();

                var agrupamento = listaBd.ToList().GroupBy(c => new { c.Ano, c.Ciclo }).Select(c => new { Ano = c.Key.Ano, Ciclo = c.Key.Ciclo });

                return Response(agrupamento.OrderByDescending(c => c.Ano).OrderByDescending(c => c.Ciclo));

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("buscar-usuarios-inscritos/{idProcessoInscricao}")]
        public async Task<IActionResult> BuscarUsuariosInscritos(int idProcessoInscricao)
        {
            try
            {
                var listaInscritos = await _inscricaoUsuarioRepository.Buscar(x => x.ProcessoInscricaoId == idProcessoInscricao);

                var retorno = listaInscritos
                    .Select(x => new
                    {
                        x.Id,
                        x.UsuarioId,
                        Nome = x.Usuario?.Nome,
                        Email = x.Usuario?.Email,
                        Cpf = x.Usuario?.Cpf,
                        TelefoneCelular = x.Usuario?.TelefoneCelular,
                        x.Status,
                        x.DataInscricao,
                        x.DataConfirmacao
                    })
                    .OrderBy(x => x.Nome)
                    .ToList();

                return Response(retorno);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("busca-com-filtro")]
        public async Task<IActionResult> BuscarComFiltro([FromBody] PaginationFilter filtro)
        {
            try
            {
                ProcessoInscricao[] processos;

                if (filtro.Filtro.Count() == 0)
                    processos = await _processoInscricaoRepository.ObterTodos();
                else
                    processos = await _processoInscricaoRepository.BuscaFiltroDinamico(filtro);

                await PreencherDadosListagem(processos);

                return Response(processos);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        private async Task PreencherDadosListagem(ProcessoInscricao[] processos)
        {
            if (!processos.Any())
                return;

            var idsProcessos = processos.Select(x => x.Id).Distinct().ToArray();

            var inscricoes = await _inscricaoUsuarioRepository.Buscar(x => idsProcessos.Contains(x.ProcessoInscricaoId));

            foreach (var processo in processos)
            {
                processo.QtdInscricoesTotal = inscricoes.Count(x => x.ProcessoInscricaoId == processo.Id);
                processo.QtdInscricoesConfirmadas = inscricoes.Count(x => x.ProcessoInscricaoId == processo.Id && x.Status == "CO");
                processo.QtdInscricoesCanceladas = inscricoes.Count(x => x.ProcessoInscricaoId == processo.Id && x.Status == "CA");
            }

            var idsSemCurso = processos.Where(x => x.Curso == null).Select(x => x.CursoId).Distinct().ToArray();

            if (!idsSemCurso.Any())
                return;

            var cursos = await _cursoRepository.Buscar(x => idsSemCurso.Contains(x.Id));

            foreach (var processo in processos.Where(x => x.Curso == null))
            {
                processo.Curso = cursos.FirstOrDefault(x => x.Id == processo.CursoId);
            }
        }

        [HttpGet("buscar-por-id/{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            try
            {
                return Response(await _processoInscricaoRepository.ObterPorId(id));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("adcionar")]
        public async Task<IActionResult> Adicionar(ProcessoInscricao processoInscricao)
        {
            try
            {
                if (!await ValidarDadosProcessoInscricao(processoInscricao))
                    return Response("Dados inválidos para cadastro.", false);

                var response = await _processoInscricaoRepository.Adicionar(processoInscricao);

                if (!response)
                    return Response("Erro ao cadastrar.", false);

                return Response("Cadastro realizado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("alterar")]
        public async Task<IActionResult> Alterar(ProcessoInscricao processoInscricao)
        {
            try
            {
                var valida = await _processoInscricaoRepository.ObterPorId(processoInscricao.Id);

                if (valida == null)
                    return Response("Id não enconrado", false);

                if (!await ValidarDadosProcessoInscricao(processoInscricao))
                    return Response("Dados inválidos para atualização.", false);

                var response = await _processoInscricaoRepository.Atualizar(processoInscricao);

                if (!response)
                    return Response("Erro ao atualizar.", false);

                return Response("Atualização realizada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        private async Task<bool> ValidarDadosProcessoInscricao(ProcessoInscricao processoInscricao)
        {
            if (processoInscricao == null)
                return false;

            if (processoInscricao.CursoId <= 0)
                return false;

            var curso = await _cursoRepository.ObterPorId(processoInscricao.CursoId);

            if (curso == null)
                return false;

            if (processoInscricao.DataInicial > processoInscricao.DataFinal)
                return false;

            if (processoInscricao.DataInicalPagto.HasValue && processoInscricao.DataFinalPagto.HasValue && processoInscricao.DataInicalPagto > processoInscricao.DataFinalPagto)
                return false;

            if (processoInscricao.DataInicioVisualizacaoCurso.HasValue && processoInscricao.DataFinalVisualizacaoCurso.HasValue && processoInscricao.DataInicioVisualizacaoCurso > processoInscricao.DataFinalVisualizacaoCurso)
                return false;

            if (processoInscricao.DataInicioPresencial.HasValue && processoInscricao.DataFinalPresencial.HasValue && processoInscricao.DataInicioPresencial > processoInscricao.DataFinalPresencial)
                return false;

            if (string.IsNullOrEmpty(processoInscricao.Status))
                processoInscricao.Status = "A";

            if (string.IsNullOrEmpty(processoInscricao.ConfiguraPeriodo))
                processoInscricao.ConfiguraPeriodo = "N";

            if (string.IsNullOrEmpty(processoInscricao.Tipo))
                processoInscricao.Tipo = "G";

            return true;
        }



    }
}
