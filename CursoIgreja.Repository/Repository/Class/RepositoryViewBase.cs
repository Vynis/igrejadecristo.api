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
    public class RepositoryViewBase<TEntity> : IRepositoryBaseView<TEntity> where TEntity : class
    {
        private readonly DataContext _dataContext;
        private readonly IFiltroDinamico _filtroDinamico;

        public RepositoryViewBase(DataContext dataContext, IFiltroDinamico filtroDinamico)
        {
            _dataContext = dataContext;
            _filtroDinamico = filtroDinamico;
        }

     
        public virtual async Task<TEntity[]> Buscar(Expression<Func<TEntity, bool>> predicado)
        {
            return await _dataContext.Set<TEntity>().Where(predicado).ToArrayAsync();
        }

        public virtual async Task<TEntity> ObterPorId(int id)
        {
            return await _dataContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity[]> ObterTodos()
        {
            return await _dataContext.Set<TEntity>().ToArrayAsync();
        }

        public virtual async Task<TEntity[]> ObterPorDescricao(string Descricao)
        {
            return await Buscar(b => b.GetType().Name.Contains("Name"));
        }

        public virtual async Task<TEntity[]> BuscaFiltroDinamico(PaginationFilter paginationFilter)
        {
            var expressionDynamic = _filtroDinamico.FromFiltroItemList<TEntity>(paginationFilter.Filtro.ToList());

            return await _dataContext.Set<TEntity>().Where(expressionDynamic).ToArrayAsync();
        }

    }
}
