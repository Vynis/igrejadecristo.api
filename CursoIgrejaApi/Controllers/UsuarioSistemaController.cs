using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("usuario-sistema")]
    public class UsuarioSistemaController : ControllerBase
    {
        private readonly IUsuarioSistemaRepository _usuarioSistemaRepository;

        public UsuarioSistemaController(IUsuarioSistemaRepository usuarioSistemaRepository)
        {
            _usuarioSistemaRepository = usuarioSistemaRepository;
        }

        [HttpGet("buscar-dados-usuario")]
        public async Task<IActionResult> BuscarUsuario()
        {
            try
            {
                var response = await _usuarioSistemaRepository.Buscar(x => x.Id == Convert.ToInt32(User.Identity.Name));

                if (!response.Any())
                    return Response("Usuario não encontrado", false);

                return Ok(new { name = response.FirstOrDefault().Nome, picture = "" });

            }
            catch (Exception ex)
            {
              return  ResponseErro(ex);
            }
        }

    }
}
