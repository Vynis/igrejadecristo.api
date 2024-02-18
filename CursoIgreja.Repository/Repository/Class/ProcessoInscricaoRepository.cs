using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Class
{
    public class ProcessoInscricaoRepository : RepositoryBase<ProcessoInscricao>, IProcessoInscricaoRepository
    {
        private readonly DataContext _dataContext;

        public ProcessoInscricaoRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<ProcessoInscricao[]> Buscar(Expression<Func<ProcessoInscricao, bool>> predicado)
        {
            IQueryable<ProcessoInscricao> query = _dataContext.ProcessoInscricoes.Where(predicado).Include(c => c.Curso);

            return await query.AsNoTracking().ToArrayAsync();
        }

        public async override Task<ProcessoInscricao[]> ObterTodos()
        {
            IQueryable<ProcessoInscricao> query = _dataContext.ProcessoInscricoes.Include(c => c.Curso);

            return await query.AsNoTracking().ToArrayAsync();
        }

        public async override Task<ProcessoInscricao> ObterPorId(int id)
        {
            IQueryable<ProcessoInscricao> query = _dataContext.ProcessoInscricoes.Include(c => c.Curso);

            return await query.Where(c => c.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }



    }
}
