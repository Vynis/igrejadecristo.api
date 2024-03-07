using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Interfaces
{
    public interface IRelatorioGeraisRepository
    {
        Task<List<dynamic>> ObterTodos(int ciclo, int ano, int processoInscricaoId); 
    }
}
