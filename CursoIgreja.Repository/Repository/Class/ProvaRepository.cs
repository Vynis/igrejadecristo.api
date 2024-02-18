using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Class
{
    public class ProvaRepository : RepositoryBase<Prova>, IProvaRepository
    {
        private readonly DataContext _dataContext;

        public ProvaRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public override async Task<Prova> ObterPorId(int id)
        {
            IQueryable<Prova> query = _dataContext.Provas.Include(c => c.ItensProvas);

            return await query.AsNoTracking().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public override Task<bool> Atualizar(Prova entity)
        {

            if (entity.TipoComponente.Equals("T"))
                return base.Atualizar(entity);

            var idItensProva = new List<int>();
            entity.ItensProvas.ForEach(x => idItensProva.Add(x.Id));
            var itensProva = _dataContext.ItemProvas.AsNoTracking().Where(x => x.ProvaId == entity.Id && !idItensProva.Contains(x.Id)).ToArray();

            if (itensProva.Length > 0)
                _dataContext.RemoveRange(itensProva);

            return base.Atualizar(entity);
        }
    }
}
