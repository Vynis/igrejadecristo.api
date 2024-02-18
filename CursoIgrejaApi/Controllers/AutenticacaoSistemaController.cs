using AutoMapper;
using CursoIgreja.Api.Dtos;
using CursoIgreja.Api.Services;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IUsuarioSistemaRepository _usuarioSistemaRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AutenticacaoSistemaController(IUsuarioSistemaRepository usuarioSistemaRepository, IConfiguration configuration, IMapper mapper)
        {
            _usuarioSistemaRepository = usuarioSistemaRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar(AutenticarDto autenticarDto)
        {
            try
            {
                autenticarDto.Senha = SenhaHashService.CalculateMD5Hash(autenticarDto.Password);

                var response = await _usuarioSistemaRepository.Buscar(x => x.Email.Equals(autenticarDto.Email)  && x.Senha.Equals(autenticarDto.Senha) && x.Status.Equals("A"));

                var usuario = _mapper.Map<UsuarioAutDto>(response.FirstOrDefault());

                if (usuario == null)
                    return BadRequest();

                var token = TokenService.GenerateToken(usuario, _configuration);

                return Response(new { usuario, token });

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }
    }
}
