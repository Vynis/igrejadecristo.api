using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Services
{
    public class GeraLogUsuario
    {
        private readonly ILogUsuarioRepository _logUsuarioRepository;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly int _idUsuario;

        public GeraLogUsuario(ILogUsuarioRepository logUsuarioRepository, IUsuariosRepository usuariosRepository, int idUsuario)
        {
            _logUsuarioRepository = logUsuarioRepository;
            _usuariosRepository = usuariosRepository;
            _idUsuario = idUsuario;
        }

        public async Task<bool> Gerar(string endPoint, string observacao = "")
        {
            try
            {
                var logUsuario = new LogUsuario();
                logUsuario.Data = DateTime.Now;
                logUsuario.UsuarioId = _idUsuario;
                logUsuario.Acao = PreparaTexto(endPoint, observacao).Result.ToString();

                var response = await _logUsuarioRepository.Adicionar(logUsuario);

                return response;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<string> PreparaTexto(string endPoint, string observacao)
        {
            try
            {
                var dadosUsuario = await _usuariosRepository.ObterPorId(_idUsuario);

                if (dadosUsuario == null)
                    return "";

                return $"Usuario: {dadosUsuario.Nome.ToUpper()}, EndPoint: {endPoint}, Obs: {observacao}";
            }
            catch (Exception)
            {
                return "";
            }

        }

    }
}
