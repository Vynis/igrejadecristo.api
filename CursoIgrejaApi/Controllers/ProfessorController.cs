using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("professor")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorRepository _professorRepository;

        public ProfessorController(IProfessorRepository professorRepository)
        {
            _professorRepository = professorRepository;
        }

        [HttpGet("buscar-ativo")]
        public async Task<IActionResult> BuscarAtivo()
        {
            try
            {
                var retorno = await _professorRepository.Buscar(x => x.Status.Equals("A"));
                return Response(retorno.OrderBy(c => c.Nome));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }
    }
}
