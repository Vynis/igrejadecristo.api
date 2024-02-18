using CursoIgreja.Api.Dtos;
using CursoIgreja.Domain.Models;
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
    [Route("prova-usuario")]
    public class ProvaUsuarioController : ControllerBase
    {
        private readonly IProvaUsuarioRepository _provaUsuarioRepository;

        public ProvaUsuarioController(IProvaUsuarioRepository provaUsuarioRepository)
        {
            _provaUsuarioRepository = provaUsuarioRepository;
        }

        [HttpPost("salvar-prova")]
        [AllowAnonymous]
        public async Task<IActionResult> SalvarProvaUsuario(RegistrarProvaUsuarioDto registrarProva)
        {
            try
            {

                registrarProva.ProvaUsuario.ForEach(x => x.UsuarioId = Convert.ToInt32(User.Identity.Name));

                foreach (var prova in registrarProva.ProvaUsuario)
                {
                    var response = await _provaUsuarioRepository.Adicionar(prova);

                    if (!response)
                        return Response("Erro adcionar prova", false);
                }

                return Response(registrarProva);
            }
            catch (Exception ex) 
            {
                return ResponseErro(ex);
            }
        }

    }
}
