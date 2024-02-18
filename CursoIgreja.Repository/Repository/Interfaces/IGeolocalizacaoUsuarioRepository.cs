using CursoIgreja.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Interfaces
{
    public interface IGeolocalizacaoUsuarioRepository : IRepositoryBase<GeolocalizacaoUsuario>
    {
        Task<bool> VerificaUsuarioEstaNoRaio(int UsuarioId);
    }
}
