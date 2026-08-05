using CursoIgreja.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Interfaces
{
   public interface ILogNotificacoesRepository : IRepositoryBase<LogNotificacao>
    {
        Task<Checkout[]> BuscarNotificaoEspecifica(string listIdInscricaoUsuario);
    }
}
