using CursoIgreja.Repository.Repository.Interfaces;
using CursoIgreja.Api.Services;
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
    [Route("usuario-sistema")]
    public class UsuarioSistemaController : ControllerBase
    {
        private readonly IUsuarioSistemaRepository _usuarioSistemaRepository;
        private readonly DataContext _dataContext;

        public UsuarioSistemaController(IUsuarioSistemaRepository usuarioSistemaRepository, DataContext dataContext)
        {
            _usuarioSistemaRepository = usuarioSistemaRepository;
            _dataContext = dataContext;
        }

        [HttpGet("buscar-dados-usuario")]
        public async Task<IActionResult> BuscarUsuario()
        {
            try
            {
                var response = await _usuarioSistemaRepository.Buscar(x => x.Id == Convert.ToInt32(User.Identity.Name));

                if (!response.Any())
                    return Response("Usuario não encontrado", false);

                var usuario = await _dataContext.UsuarioSistemas
                    .Include(x => x.UsuarioPerfis)
                    .ThenInclude(x => x.Perfil)
                    .ThenInclude(x => x.PerfilPermissoes)
                    .ThenInclude(x => x.Permissoes)
                    .FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(User.Identity.Name));

                return Ok(new
                {
                    name = usuario.Nome,
                    picture = "",
                    perfis = usuario.UsuarioPerfis?.Select(x => x.Perfil?.Titulo).Where(x => x != null).ToList(),
                    permissoes = ObterPermissoes(usuario),
                    administrador = UsuarioAdministrador(usuario)
                });

            }
            catch (Exception ex)
            {
              return  ResponseErro(ex);
            }
        }

        [HttpGet("buscar-todos")]
        public async Task<IActionResult> BuscarTodos()
        {
            try
            {
                if (!await UsuarioTemPermissao("usuariosistema.visualizar"))
                    return Unauthorized();

                return Response(await ListarUsuariosSistema());
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("busca-com-filtro")]
        public async Task<IActionResult> BuscarComFiltro([FromBody] PaginationFilter filtro)
        {
            try
            {
                if (!await UsuarioTemPermissao("usuariosistema.visualizar"))
                    return Unauthorized();

                var query = _dataContext.UsuarioSistemas
                    .Include(x => x.UsuarioPerfis)
                    .ThenInclude(x => x.Perfil)
                    .AsQueryable();

                foreach (var item in filtro.Filtro)
                {
                    var valor = item.Value?.ToString() ?? string.Empty;

                    if (item.Property.Equals("Nome", StringComparison.OrdinalIgnoreCase))
                        query = query.Where(x => x.Nome.Contains(valor));

                    if (item.Property.Equals("Email", StringComparison.OrdinalIgnoreCase))
                        query = query.Where(x => x.Email.Contains(valor));

                    if (item.Property.Equals("Status", StringComparison.OrdinalIgnoreCase))
                        query = query.Where(x => x.Status.Equals(valor));
                }

                return Response((await query.ToListAsync()).Select(MapearUsuarioSistema));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("busca-por-id/{id}")]
        public async Task<IActionResult> BuscaPorId(int id)
        {
            try
            {
                if (!await UsuarioTemPermissao("usuariosistema.visualizar"))
                    return Unauthorized();

                var usuario = await _dataContext.UsuarioSistemas
                    .Include(x => x.UsuarioPerfis)
                    .ThenInclude(x => x.Perfil)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (usuario == null)
                    return Response("Id não encontrado", false);

                return Response(MapearUsuarioSistema(usuario));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("perfis")]
        public async Task<IActionResult> BuscarPerfis()
        {
            try
            {
                if (!await UsuarioTemPermissao("usuariosistema.visualizar"))
                    return Unauthorized();

                return Response(await _dataContext.Perfis.OrderBy(x => x.Titulo).ToListAsync());
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar(UsuarioSistema usuario)
        {
            try
            {
                if (!await UsuarioTemPermissao("usuariosistema.criar"))
                    return Unauthorized();

                var verficaCadastro = await _dataContext.UsuarioSistemas.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(usuario.Email.ToLower()));

                if (verficaCadastro != null)
                    return Response("Cadastro já se encontra na base de dados!", false);

                usuario.Id = 0;
                usuario.Senha = SenhaHashService.CalculateMD5Hash("123456");
                usuario.Status = string.IsNullOrEmpty(usuario.Status) ? "A" : usuario.Status;

                var perfilIds = usuario.UsuarioPerfis?.Select(x => x.PerfilId).ToList() ?? new List<int>();
                usuario.UsuarioPerfis = perfilIds.Select(x => new UsuarioPerfis { PerfilId = x }).ToList();

                var response = await _usuarioSistemaRepository.Adicionar(usuario);

                if (response)
                    return Response(MapearUsuarioSistema(usuario));

                return Response("Cadastro não realizado", false);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("alterar")]
        public async Task<IActionResult> Alterar(UsuarioSistema usuario)
        {
            try
            {
                if (!await UsuarioTemPermissao("usuariosistema.editar"))
                    return Unauthorized();

                var valida = await _dataContext.UsuarioSistemas
                    .Include(x => x.UsuarioPerfis)
                    .FirstOrDefaultAsync(x => x.Id == usuario.Id);

                if (valida == null)
                    return Response("Id não encontrado", false);

                var emailExiste = await _dataContext.UsuarioSistemas.AnyAsync(x => x.Id != usuario.Id && x.Email.ToLower().Equals(usuario.Email.ToLower()));

                if (emailExiste)
                    return Response("Email já se encontra na base de dados!", false);

                valida.Nome = usuario.Nome;
                valida.Email = usuario.Email;
                valida.Status = usuario.Status;

                var perfilIds = usuario.UsuarioPerfis?.Select(x => x.PerfilId).Distinct().ToList() ?? new List<int>();
                _dataContext.UsuarioPerfis.RemoveRange(valida.UsuarioPerfis);
                valida.UsuarioPerfis = perfilIds.Select(x => new UsuarioPerfis { UsuarioSistemaId = usuario.Id, PerfilId = x }).ToList();

                var response = await _dataContext.SaveChangesAsync() > 0;

                if (!response)
                    return Response("Erro ao atualizar.", false);

                return Response("Atualização realizada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("resetar-senha/{id}")]
        public async Task<IActionResult> ResetarSenha(int id)
        {
            try
            {
                if (!await UsuarioTemPermissao("usuariosistema.resetar_senha"))
                    return Unauthorized();

                var valida = await _usuarioSistemaRepository.ObterPorId(id);

                if (valida == null)
                    return Response("Id não encontrado", false);

                valida.Senha = SenhaHashService.CalculateMD5Hash("123456");

                var response = await _usuarioSistemaRepository.Atualizar(valida);

                if (!response)
                    return Response("Erro ao resetar senha.", false);

                return Response("Senha resetada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        private async Task<IEnumerable<object>> ListarUsuariosSistema()
        {
            var usuarios = await _dataContext.UsuarioSistemas
                .Include(x => x.UsuarioPerfis)
                .ThenInclude(x => x.Perfil)
                .OrderBy(x => x.Nome)
                .ToListAsync();

            return usuarios.Select(MapearUsuarioSistema);
        }

        private object MapearUsuarioSistema(UsuarioSistema usuario)
        {
            return new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Status,
                PerfilIds = usuario.UsuarioPerfis?.Select(x => x.PerfilId).ToList(),
                Perfis = usuario.UsuarioPerfis?.Select(x => x.Perfil?.Titulo).Where(x => x != null).ToList()
            };
        }

        private async Task<bool> UsuarioTemPermissao(string permissao)
        {
            var usuario = await ObterUsuarioLogadoComPermissoes();

            if (UsuarioAdministrador(usuario))
                return true;

            return ObterPermissoes(usuario).Contains(permissao);
        }

        private async Task<UsuarioSistema> ObterUsuarioLogadoComPermissoes()
        {
            if (!int.TryParse(User.Identity.Name, out var usuarioId))
                return null;

            return await _dataContext.UsuarioSistemas
                .Include(x => x.UsuarioPerfis)
                .ThenInclude(x => x.Perfil)
                .ThenInclude(x => x.PerfilPermissoes)
                .ThenInclude(x => x.Permissoes)
                .FirstOrDefaultAsync(x => x.Id == usuarioId);
        }

        private List<string> ObterPermissoes(UsuarioSistema usuario)
        {
            if (UsuarioAdministrador(usuario))
                return new List<string> { "*" };

            return usuario?.UsuarioPerfis?
                .SelectMany(x => x.Perfil?.PerfilPermissoes ?? new List<PerfilPermissoes>())
                .Select(x => x.Permissoes?.Chave)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList() ?? new List<string>();
        }

        private async Task<bool> UsuarioLogadoAdministrador()
        {
            if (!int.TryParse(User.Identity.Name, out var usuarioId))
                return false;

            var usuario = await _dataContext.UsuarioSistemas
                .Include(x => x.UsuarioPerfis)
                .ThenInclude(x => x.Perfil)
                .FirstOrDefaultAsync(x => x.Id == usuarioId);

            return UsuarioAdministrador(usuario);
        }

        private bool UsuarioAdministrador(UsuarioSistema usuario)
        {
            return usuario?.UsuarioPerfis?.Any(x => x.Perfil != null && x.Perfil.Titulo.Equals("Administrador", StringComparison.OrdinalIgnoreCase)) == true;
        }

    }
}
