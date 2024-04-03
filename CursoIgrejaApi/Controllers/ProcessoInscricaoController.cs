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
        private readonly IMapper _mapper;
        private readonly IInscricaoUsuarioRepository _inscricaoUsuarioRepository;

        public ProcessoInscricaoController(IProcessoInscricaoRepository processoInscricaoRepository, IMapper mapper, IInscricaoUsuarioRepository inscricaoUsuarioRepository)
        {
            _processoInscricaoRepository = processoInscricaoRepository;
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



    }
}
