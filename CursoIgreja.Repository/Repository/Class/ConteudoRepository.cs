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
    public class ConteudoRepository : RepositoryBase<Conteudo>, IConteudoRepository
    {
        private readonly DataContext _dataContext;

        public ConteudoRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<Conteudo[]> Buscar(Expression<Func<Conteudo, bool>> predicado)
        {
            IQueryable<Conteudo> query = _dataContext.Conteudos.Where(predicado)
                                    .Include(c => c.Provas)
                                    .Include(c => c.Modulo)
                                    .Include(c => c.Modulo.Curso);

            return await query.AsNoTracking().ToArrayAsync();
        }

        public async override Task<Conteudo> ObterPorId(int id)
        {
            IQueryable<Conteudo> query = _dataContext.Conteudos
                                                .Include(c => c.Provas)
                                                .Include("Provas.ItensProvas")
                                                .Include("Provas.ProvaUsuarios")
                                                .Include(c => c.Modulo)
                                                .Include(c => c.Modulo.Curso);

            return await query.AsNoTracking().Where(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}
