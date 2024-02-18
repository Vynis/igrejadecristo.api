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
    [Route("conteudo")]
    public class ConteudoController : ControllerBase
    {
        private readonly IConteudoRepository _conteudoRepository;

        public ConteudoController(IConteudoRepository conteudoRepository)
        {
            _conteudoRepository = conteudoRepository;
        }


        [HttpGet("buscar-conteudo/{idCurso}")]
        public async Task<IActionResult> BuscaTodosConteudosCurso(int idCurso)
        {
            try
            {
                var response = await _conteudoRepository.Buscar(x => x.Modulo.CursoId == idCurso);
                return Response(response.OrderBy(c => c.Modulo.Ordem));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("buscar-id/{id}")]
        public async Task<IActionResult> BuscaConteudoId(int id)
        {
            try
            {
                var response = await _conteudoRepository.ObterPorId(id);
                return Response(response);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("adcionar")]
        public async Task<IActionResult> Add(Conteudo objeto)
        {
            try
            {
                var response = await _conteudoRepository.Adicionar(objeto);

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
        public async Task<IActionResult> Alt(Conteudo objeto)
        {
            try
            {
                var valida = await _conteudoRepository.ObterPorId(objeto.Id);

                if (valida == null)
                    return Response("Id não enconrado", false);

                var response = await _conteudoRepository.Atualizar(objeto);

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
                var valida = await _conteudoRepository.ObterPorId(id);

                if (valida == null)
                    return Response("Id não enconrado", false);

                if (valida.Provas.Any())
                    return Response("Não foi possível excluir possuir vinculo com provas!", false);

                var response = await _conteudoRepository.Remover(valida);

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
