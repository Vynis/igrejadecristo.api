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
    [Route("conteudo-usuario")]
    public class ConteudoUsuarioController : ControllerBase
    {
        private readonly IConteudoUsuarioRepository _conteudoUsuarioRepository;

        public ConteudoUsuarioController(IConteudoUsuarioRepository conteudoUsuarioRepository)
        {
            _conteudoUsuarioRepository = conteudoUsuarioRepository;
        }

        [HttpGet("busca-conteudo-usuario/{id}")]
        public async Task<IActionResult> BuscaConteudoUsuario(int id)
        {
            try
            {
                var retorno = await _conteudoUsuarioRepository.Buscar(x => x.UsuariosId == Convert.ToInt32(User.Identity.Name) && x.Conteudo.Modulo.CursoId == id );

                return Response(retorno);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("salvar-conteudo-usuario")]
        public async Task<IActionResult> Salvar(ConteudoUsuario conteudoUsuario)
        {
            try
            {
                var valida = await _conteudoUsuarioRepository.Buscar(x => x.ConteudoId == conteudoUsuario.ConteudoId && x.UsuariosId == Convert.ToInt32(User.Identity.Name));

                if (valida.Any())
                    return Response("Item ja salvo", false);

                conteudoUsuario.DataConclusao = DateTime.Now;
                conteudoUsuario.UsuariosId = Convert.ToInt32(User.Identity.Name);
                conteudoUsuario.Concluido = "S";

                var retorno = await _conteudoUsuarioRepository.Adicionar(conteudoUsuario);

                if (!retorno)
                        return Response("Erro ao salvar", false);

                return Response("Salvo com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }
    }
}
