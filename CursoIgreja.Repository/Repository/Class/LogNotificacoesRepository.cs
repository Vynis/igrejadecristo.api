using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Class
{
    public class LogNotificacoesRepository : RepositoryBase<LogNotificacao>, ILogNotificacoesRepository
    {
        private readonly DataContext _dataContext;

        public LogNotificacoesRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<LogNotificacao[]> Buscar(Expression<Func<LogNotificacao, bool>> predicado)
        {
            IQueryable<LogNotificacao> query = _dataContext.LogNotificacoes.Where(predicado);

            return await query.AsNoTracking().ToArrayAsync();

        }
    }
}
