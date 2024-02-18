using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("prova")]
    public class ProvaController : ControllerBase
    {
        private readonly IProvaRepository _provaRepository;

        public ProvaController(IProvaRepository provaRepository)
        {
            _provaRepository = provaRepository;
        }

        [HttpGet("buscar-prova/{idConteudo}")]
        public async Task<IActionResult> BuscaTodosProvaConteudo(int idConteudo)
        {
            try
            {
                var response = await _provaRepository.Buscar(x => x.ConteudoId == idConteudo);
                return Response(response.OrderBy(c => c.Ordem));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("buscar-id/{id}")]
        public async Task<IActionResult> BuscaProvaId(int id)
        {
            try
            {
                var response = await _provaRepository.ObterPorId(id);
                return Response(response);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("adcionar")]
        public async Task<IActionResult> Add(Prova objeto)
        {
            try
            {
                var response = await _provaRepository.Adicionar(objeto);

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
        public async Task<IActionResult> Alt(Prova objeto)
        {
            try
            {
                var valida = await _provaRepository.ObterPorId(objeto.Id);

                if (valida == null)
                    return Response("Id não enconrado", false);

                var response = await _provaRepository.Atualizar(objeto);

                if (!response)
                    return Response("Erro ao atualizar.", false);

                return Response("Atualização realizada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpDelete("deletar/{id}")]
        public async Task<IActionResult> Del(int id)
        {
            try
            {
                var valida = await _provaRepository.ObterPorId(id);

                if (valida == null)
                    return Response("Id não enconrado", false);

                if (valida.ItensProvas.Any())
                    return Response("Não foi possível excluir possuir vinculo com itens de provas!", false);

                var response = await _provaRepository.Remover(valida);

                if (!response)
                    return Response("Erro ao deletar.", false);

                return Response("Atualização realizada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }
    }
}
