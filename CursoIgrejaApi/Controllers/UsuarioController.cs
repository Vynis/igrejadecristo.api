using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CursoIgreja.Api.Services;
using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuariosRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioController(IUsuariosRepository usuariosRepository, IMapper mapper)
        {
            _usuarioRepository = usuariosRepository;
            _mapper = mapper;
        }

        [HttpGet("busca-por-id/{id}")]
        public async Task<IActionResult> BuscaPorId(int id)
        {
            try
            {
                return Response(await _usuarioRepository.ObterPorId(id));
            }
            catch (Exception ex)
            {

                return ResponseErro(ex);
            }
        }

        [HttpPost("cadastrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Cadastrar(Usuarios usuario)
        {
            try
            {
                //Valida se usuario já existe no banco
                var verficaCadastro = new Usuarios();

                if (!string.IsNullOrEmpty(usuario.Email))
                    verficaCadastro = _usuarioRepository.Buscar(x => x.Email.ToLower().Equals(usuario.Email.ToLower())).Result.FirstOrDefault();
                else
                    verficaCadastro = _usuarioRepository.Buscar(x => x.Cpf.Equals(usuario.Cpf)).Result.FirstOrDefault();

                if (verficaCadastro != null)
                    return Response("Cadastro já se encontra na base de dados!", false);

                usuario.Senha = SenhaHashService.CalculateMD5Hash(usuario.Senha);

                //Salva os dados
                var response = await _usuarioRepository.Adicionar(usuario);

                if (response)
                    return Response(usuario);

                return Response("Cadastro não realizado", false);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("alterar")]
        public async Task<IActionResult> Alterar(Usuarios usuario)
        {
            try
            {
                var valida = await _usuarioRepository.ObterPorId(usuario.Id);

                if (valida == null)
                    return Response("Id não enconrado", false);

                var response = await _usuarioRepository.Atualizar(usuario);

                if (!response)
                    return Response("Erro ao atualizar.", false);

                return Response("Atualização realizada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("verifica-cadastro/{emailOuCpf}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerificaCadastro(string emailOuCpf) 
        {
            try
            {
                var verficaCadastro = new Usuarios();

                verficaCadastro = _usuarioRepository.Buscar(x => x.Email.Equals(emailOuCpf)).Result.FirstOrDefault();

                if (verficaCadastro != null)
                    return Response("Cadastro já se encontra na base de dados!", false);

                verficaCadastro = _usuarioRepository.Buscar(x => x.Cpf.Equals(emailOuCpf)).Result.FirstOrDefault();

                if (verficaCadastro != null)
                    return Response("Cadastro já se encontra na base de dados!", false);

                return Response(true);

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

    }
}
