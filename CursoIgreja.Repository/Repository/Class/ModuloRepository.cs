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
    public class ModuloRepository : RepositoryBase<Modulo>, IModuloRepository
    {
        private readonly DataContext _dataContext;

        public ModuloRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<Modulo[]> Buscar(Expression<Func<Modulo, bool>> predicado)
        {
            IQueryable<Modulo> query = _dataContext.Modulos
                                                                .Where(predicado)
                                                                .Include(c => c.LiberacaoModulos)
                                                                .Include(c => c.Conteudos)
                                                                .Include("Conteudos.Anexos")
                                                                .Include("Conteudos.ConteudoUsuarios")
                                                                .Include("Conteudos.Provas")
                                                                .Include("Conteudos.Provas.ItensProvas")
                                                                .Include("Conteudos.Provas.ProvaUsuarios")
                                                                .Include(c => c.Curso);

            return await query.AsNoTracking().OrderBy(c => c.Ordem).ToArrayAsync();
        }

        public async override Task<Modulo[]> ObterTodos()
        {
            IQueryable<Modulo> query = _dataContext.Modulos
                                                                .Include(c => c.Conteudos)
                                                                .Include(c => c.Curso);

            return await query.AsNoTracking().OrderBy(c => c.Ordem).ToArrayAsync();
        }

        public async override Task<Modulo> ObterPorId(int id)
        {
            IQueryable<Modulo> query = _dataContext.Modulos
                                                                .Include(c => c.Conteudos)
                                                                .Include(c => c.Curso);

            return await query.AsTracking().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

    }
}
