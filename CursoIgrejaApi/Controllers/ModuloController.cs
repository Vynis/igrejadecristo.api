using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("modulo")]
    public class ModuloController : ControllerBase
    {
        private readonly IModuloRepository _moduloRepository;
        private readonly IProvaUsuarioRepository _provaUsuarioRepository;

        public ModuloController(IModuloRepository moduloRepository, IProvaUsuarioRepository provaUsuarioRepository)
        {
            _moduloRepository = moduloRepository;
            _provaUsuarioRepository = provaUsuarioRepository;
        }


        [HttpGet("buscar-modulos")]
        public async Task<IActionResult> BuscarModulos()
        {
            try
            {
                var modulos = await _moduloRepository.ObterTodos();

                return Response(modulos);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }


        [HttpGet("buscar-modulo-curso/{idCurso}")]
        public async Task<IActionResult> BuscarModuloCurso(int idCurso)
        {
            try
            {
                var retorno = await _moduloRepository.Buscar(x => x.CursoId.Equals(idCurso));

                var listaProvaUsuario = await _provaUsuarioRepository.Buscar(x => x.UsuarioId.Equals(Convert.ToInt32(User.Identity.Name)));

                foreach (var modulo in retorno)
                    foreach(var conteudo in modulo.Conteudos)
                    {   
                        if (conteudo.Tipo.Equals("PR") || conteudo.Tipo.Equals("PA"))
                        {
                            var provaUsuario = listaProvaUsuario.Where(x => x.Prova.ConteudoId.Equals(conteudo.Id)).ToList();

                            if (provaUsuario.Count > 0)
                                conteudo.ConteudoConcluido = true;
                            else
                                conteudo.ConteudoConcluido = false;
                        }
                        else
                            conteudo.ConteudoConcluido = conteudo.ConteudoUsuarios.Exists(x => x.ConteudoId == conteudo.Id && x.UsuariosId == Convert.ToInt32(User.Identity.Name) && x.Concluido.Equals("S"));
                    }

                return Response(retorno);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }


    }
}
