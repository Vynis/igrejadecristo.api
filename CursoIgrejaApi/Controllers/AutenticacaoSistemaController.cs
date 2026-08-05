using AutoMapper;
using CursoIgreja.Api.Dtos;
using CursoIgreja.Api.Services;
using CursoIgreja.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("authadmin")]
    public class AutenticacaoSistemaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        public AutenticacaoSistemaController(IConfiguration configuration, IMapper mapper, DataContext dataContext)
        {
            _configuration = configuration;
            _mapper = mapper;
            _dataContext = dataContext;
        }

        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar(AutenticarDto autenticarDto)
        {
            try
            {
                autenticarDto.Senha = SenhaHashService.CalculateMD5Hash(autenticarDto.Password);

                var response = await _dataContext.UsuarioSistemas
                    .Include(x => x.UsuarioPerfis)
                    .ThenInclude(x => x.Perfil)
                    .ThenInclude(x => x.PerfilPermissoes)
                    .ThenInclude(x => x.Permissoes)
                    .FirstOrDefaultAsync(x => x.Email.Equals(autenticarDto.Email) && x.Senha.Equals(autenticarDto.Senha) && x.Status.Equals("A"));

                var usuario = _mapper.Map<UsuarioAutDto>(response);

                if (usuario == null)
                    return BadRequest();

                var token = TokenService.GenerateToken(usuario, _configuration);

                return Response(new
                {
                    usuario,
                    token,
                    perfis = response.UsuarioPerfis?.Select(x => x.Perfil?.Titulo).Where(x => x != null).ToList(),
                    permissoes = ObterPermissoes(response),
                    administrador = UsuarioAdministrador(response)
                });

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        private List<string> ObterPermissoes(CursoIgreja.Domain.Models.UsuarioSistema usuario)
        {
            if (UsuarioAdministrador(usuario))
                return new List<string> { "*" };

            return usuario.UsuarioPerfis?
                .SelectMany(x => x.Perfil?.PerfilPermissoes ?? new List<CursoIgreja.Domain.Models.PerfilPermissoes>())
                .Select(x => x.Permissoes?.Chave)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList() ?? new List<string>();
        }

        private bool UsuarioAdministrador(CursoIgreja.Domain.Models.UsuarioSistema usuario)
        {
            return usuario?.UsuarioPerfis?.Any(x => x.Perfil != null && x.Perfil.Titulo.Equals("Administrador", StringComparison.OrdinalIgnoreCase)) == true;
        }
    }
}
