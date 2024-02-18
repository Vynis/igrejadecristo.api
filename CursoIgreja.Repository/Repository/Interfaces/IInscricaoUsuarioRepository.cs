using CursoIgreja.Domain.Models;
using CursoIgreja.PagSeguro.TransferObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Interfaces
{
    public interface IInscricaoUsuarioRepository : IRepositoryBase<InscricaoUsuario>
    {
        Task<bool> AtualizaGeral(List<ConsultaTransacaoPagSeguroTransactionDTO> listaNotificacoes, int idProcessoInscricao);
    }
}
