using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("congregacao")]
    public class CongregacaoController : ControllerBase
    {
        private readonly ICongregacaoRepository _congregacaoRepository;

        public CongregacaoController(ICongregacaoRepository congregacaoRepository)
        {
           _congregacaoRepository = congregacaoRepository;
        }

        [HttpGet("buscar-todos-ativos")]
        [AllowAnonymous]
        public async Task<IActionResult> BuscarTodosAtivos()
        {
            try
            {
                var listaCongregacoes = await _congregacaoRepository.Buscar(x => x.Status.Equals("A"));
                return Response(listaCongregacoes);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

    }
}
