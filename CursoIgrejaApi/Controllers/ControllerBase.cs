using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CursoIgreja.Api.Controllers
{
    public class ControllerBase : Controller
    {
        protected new IActionResult Response(object result = null, bool success = true)
        {
            return Ok(new { success = success, dados = result });
        }

        protected IActionResult ResultadoPesquisa<T>(IEnumerable<T> Dados)
        {
            return Ok(new { success = true, lista = Dados, quantidade = Dados.Count() });
        }

        protected IActionResult ResutladoPesquisa(IEnumerable<object> lista)
        {
            return Ok(new { success = true, dados = lista, quantidade = lista.Count() });
        }

        protected IActionResult ResponseErro(Exception Ex)
        {
            return BadRequest(new
            {
                sucess = false,
                Erro = Ex.Message.ToString(),
                Stack = Ex.StackTrace,
                StatusErro = $"{StatusCodes.Status500InternalServerError} - Erro ao processar a requisição"
            });
        }

        protected IActionResult ResponseNotFount(string Mensagem)
        {
            return NotFound(new { success = false, Message = Mensagem });
        }
    }
}
