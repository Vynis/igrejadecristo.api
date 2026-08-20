using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;

namespace CursoIgreja.Repository.Repository.Class
{
    public class PequenoGrupoRelatorioPresencaRepository : RepositoryBase<PequenoGrupoRelatorioPresenca>, IPequenoGrupoRelatorioPresencaRepository
    {
        public PequenoGrupoRelatorioPresencaRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
        }
    }
}
