using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;

namespace CursoIgreja.Repository.Repository.Class
{
    public class LiderPequenoGrupoRepository : RepositoryBase<LiderPequenoGrupo>, ILiderPequenoGrupoRepository
    {
        public LiderPequenoGrupoRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
        }
    }
}
