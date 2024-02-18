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
    [Route("membro")]
    public class MembroController : ControllerBase
    {
        private readonly IMembroRepository _membroRepository;

        public MembroController(IMembroRepository membroRepository)
        {
            _membroRepository = membroRepository;
        }

        [HttpPost("adcionar")]
        [AllowAnonymous]
        public async Task<IActionResult> Add(Membro membro)
        {
            try
            {
                if (membro.PossuiEmail)
                {

                    if (string.IsNullOrEmpty(membro.Email))
                        return Response("Informe o email!", false);

                    //Valida email existente
                    var validaEmail = await _membroRepository.Buscar(x => x.Email.Trim().ToUpper().Equals(membro.Email.Trim().ToUpper()));

                    if (validaEmail.Any())
                        return Response("Email já cadastrado na base de dados!", false);
                }

                //Valida nome existente
                var validaNome = await _membroRepository.Buscar(x => x.Nome.Trim().ToUpper().Equals(membro.Nome.Trim().ToUpper()));

                if (validaNome.Any())
                    return Response("Nome já cadastrado na base de dados!", false);

                var response = await _membroRepository.Adicionar(membro);

                if (!response)
                    return Response("Erro ao cadastrar.", false);

                return Response("Cadastro realizado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }
    }
}
