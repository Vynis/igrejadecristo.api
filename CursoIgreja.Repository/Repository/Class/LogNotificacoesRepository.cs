using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public async Task<Checkout[]> BuscarNotificaoEspecifica(string listIdInscricaoUsuario)
        {
            var resut = await _dataContext.LogNotificacoes.FromSqlRaw($@"SELECT * FROM lognotificacoes lon WHERE JSON_UNQUOTE(JSON_EXTRACT(lon.xml, '$.reference_id')) in ({listIdInscricaoUsuario})").ToArrayAsync();

            var list = new List<Checkout>();

            foreach (var item in resut) { 
                list.Add(JsonConvert.DeserializeObject<Checkout>(item.Xml));
            }

            return list.ToArray();
        }
    }
}
