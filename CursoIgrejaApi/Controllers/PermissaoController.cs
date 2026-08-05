using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("permissao")]
    public class PermissaoController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public PermissaoController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("perfis")]
        public async Task<IActionResult> BuscarPerfis()
        {
            try
            {
                if (!await UsuarioLogadoAdministrador())
                    return Unauthorized();

                return Response(await _dataContext.Perfis.OrderBy(x => x.Titulo).ToListAsync());
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("buscar-todas")]
        public async Task<IActionResult> BuscarTodas()
        {
            try
            {
                if (!await UsuarioLogadoAdministrador())
                    return Unauthorized();

                return Response(await _dataContext.Permissoes.OrderBy(x => x.Titulo).ToListAsync());
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("perfil/{perfilId}")]
        public async Task<IActionResult> BuscarPermissoesPerfil(int perfilId)
        {
            try
            {
                if (!await UsuarioLogadoAdministrador())
                    return Unauthorized();

                var permissoes = await _dataContext.PerfilPermissoes
                    .Where(x => x.PefilId == perfilId)
                    .Select(x => x.PermissoesId)
                    .ToListAsync();

                return Response(permissoes);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("salvar-perfil")]
        public async Task<IActionResult> SalvarPermissoesPerfil([FromBody] SalvarPermissoesPerfilDto dto)
        {
            try
            {
                if (!await UsuarioLogadoAdministrador())
                    return Unauthorized();

                var perfil = await _dataContext.Perfis.FirstOrDefaultAsync(x => x.Id == dto.PerfilId);

                if (perfil == null)
                    return Response("Perfil não encontrado", false);

                var permissoesAtuais = await _dataContext.PerfilPermissoes
                    .Where(x => x.PefilId == dto.PerfilId)
                    .ToListAsync();

                _dataContext.PerfilPermissoes.RemoveRange(permissoesAtuais);

                var permissoesIds = dto.PermissoesIds?.Distinct().ToList() ?? new List<int>();

                foreach (var permissaoId in permissoesIds)
                {
                    _dataContext.PerfilPermissoes.Add(new PerfilPermissoes
                    {
                        PefilId = dto.PerfilId,
                        PermissoesId = permissaoId
                    });
                }

                await _dataContext.SaveChangesAsync();

                return Response("Permissões atualizadas com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        private async Task<bool> UsuarioLogadoAdministrador()
        {
            if (!int.TryParse(User.Identity.Name, out var usuarioId))
                return false;

            var usuario = await _dataContext.UsuarioSistemas
                .Include(x => x.UsuarioPerfis)
                .ThenInclude(x => x.Perfil)
                .FirstOrDefaultAsync(x => x.Id == usuarioId);

            return usuario?.UsuarioPerfis?.Any(x => x.Perfil != null && x.Perfil.Titulo.Equals("Administrador", StringComparison.OrdinalIgnoreCase)) == true;
        }
    }

    public class SalvarPermissoesPerfilDto
    {
        public int PerfilId { get; set; }
        public List<int> PermissoesIds { get; set; }
    }
}
