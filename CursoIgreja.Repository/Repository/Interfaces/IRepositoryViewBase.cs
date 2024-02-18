using CursoIgreja.Domain.Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Interfaces
{
    public interface IRepositoryBaseView<TEntity> where TEntity : class
    {
        Task<TEntity[]> ObterTodos();

        Task<TEntity> ObterPorId(int id);

        Task<TEntity[]> ObterPorDescricao(string Descricao);

        Task<TEntity[]> Buscar(Expression<Func<TEntity, bool>> predicado);

        Task<TEntity[]> BuscaFiltroDinamico(PaginationFilter paginationFilter);

    }
}
