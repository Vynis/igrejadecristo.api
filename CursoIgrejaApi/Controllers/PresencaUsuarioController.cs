using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("presenca-usuario")]
    public class PresencaUsuarioController : ControllerBase
    {
        private readonly IPresencaUsuarioRepository _presencaUsuarioRepository;
        private readonly IVwPresencaUsuarioRepository _vwPresencaUsuarioRepository;
        private readonly ICalendarioAulasRepository _calendarioAulasRepository;

        public PresencaUsuarioController(IPresencaUsuarioRepository presencaUsuarioRepository, IVwPresencaUsuarioRepository vwPresencaUsuarioRepository, ICalendarioAulasRepository calendarioAulasRepository )
        {
            _presencaUsuarioRepository = presencaUsuarioRepository;
            _vwPresencaUsuarioRepository = vwPresencaUsuarioRepository;
            _calendarioAulasRepository = calendarioAulasRepository;
        }

        [HttpGet("buscar-aulas-disponiveis")]
        public async Task<IActionResult> BuscarAulasDisponiveis()
        {
            try
            {
                var response = await _calendarioAulasRepository.Buscar(x => x.DataAula.Date <= DateTime.Now.Date && x.Recesso.Equals("N"));

                var listaGroup = response.GroupBy(x => x.DataAula.Date).Select(x => x.First()).ToList();

                return Response(listaGroup.OrderByDescending(x => x.DataAula.Date));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("buscar-todos-presenca-usuario/{data}")]
        public async Task<IActionResult> BuscarTodosPresencaUsuario(DateTime data)
        {
            try
            {
                var response = await _vwPresencaUsuarioRepository.Buscar(x => x.DataAula.Date == data.Date);

                return Response(response);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("inserir-presenca-usuario")]
        public async Task<IActionResult> InserirPresencaUsuario(PresencaUsuario presencaUsuario)
        {
            try
            {
                var response = await _presencaUsuarioRepository.Adicionar(presencaUsuario);

                return Response();
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpDelete("deletar-presenca-usuario/{usuarioId}/{processoInscricaoId}/{data}")]
        public async Task<IActionResult> DeletarPresencaUsuario(int usuarioId, int processoInscricaoId, DateTime data)
        {
            try
            {

                var obterCheckIn = await _presencaUsuarioRepository.Buscar(x => x.UsuarioId == usuarioId && x.ProcessoInscricaoId == processoInscricaoId && x.DataRegistro.Date == data.Date);

                if (!obterCheckIn.Any())
                    return Response(null, false);

                var response = await _presencaUsuarioRepository.Remover(obterCheckIn.FirstOrDefault());

                return Response();
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

    }
}
