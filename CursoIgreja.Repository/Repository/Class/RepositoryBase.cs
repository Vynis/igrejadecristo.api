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
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly DataContext _dataContext;
        private readonly IFiltroDinamico _filtroDinamico;

        public RepositoryBase(DataContext dataContext, IFiltroDinamico filtroDinamico)
        {
            _dataContext = dataContext;
            _filtroDinamico = filtroDinamico;
        }

        public virtual async Task<bool> SaveChangesAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Adicionar(TEntity entity)
        {
            _dataContext.Add(entity);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Atualizar(TEntity entity)
        {
            _dataContext.Update(entity);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> AtualizarRange(TEntity[] entity)
        {
            _dataContext.UpdateRange(entity);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Remover(TEntity entity)
        {
            _dataContext.Remove(entity);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> RemoverRange(TEntity[] entity)
        {
            _dataContext.RemoveRange(entity);
            return await _dataContext.SaveChangesAsync() > 0;
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

        public virtual void DeatchLocal(Func<TEntity, bool> predicado)
        {
            var local = _dataContext.Set<TEntity>().Local.Where(predicado).FirstOrDefault();
            if (local != null)
            {
                _dataContext.Entry(local).State = EntityState.Detached;
            }
        }
    }
}
