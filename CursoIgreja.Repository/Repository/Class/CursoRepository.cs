using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Class
{
    public class CursoRepository : RepositoryBase<Curso>, ICursoRepository
    {
        private readonly DataContext _dataContext;

        public CursoRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<Curso[]> Buscar(Expression<Func<Curso, bool>> predicado)
        {
            IQueryable<Curso> query = _dataContext.Cursos.Where(predicado).Include(c => c.Modulo);

            return await query.AsNoTracking().ToArrayAsync();
        }

        public async override Task<Curso[]> ObterTodos()
        {
            IQueryable<Curso> query = _dataContext.Cursos.Include(c => c.Modulo);

            return await query.AsNoTracking().ToArrayAsync();
        }

        public async override Task<Curso> ObterPorId(int id)
        {
            IQueryable<Curso> query = _dataContext.Cursos.Include(c => c.Modulo).Include(c => c.CursoProfessores).Include("CursoProfessores.Professor");

            return await query.AsNoTracking().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public override Task<bool> Atualizar(Curso entity)
        {
            var idModulos = new List<int>();
            entity.Modulo.ForEach(x => idModulos.Add(x.Id));
            var modulos = _dataContext.Modulos.AsNoTracking().Where(modulo => modulo.CursoId == entity.Id  && !idModulos.Contains(modulo.Id)).ToArray();

            if (modulos.Length > 0)
                _dataContext.RemoveRange(modulos);

            var idProfessores = new List<int>();
            entity.CursoProfessores.ForEach(x => idProfessores.Add(x.ProfessorId));
            var curso = _dataContext.CursoProfessores.AsNoTracking().Where(curso => curso.CursoId == entity.Id && !idProfessores.Contains(curso.ProfessorId)).ToArray();

            if (curso.Length > 0)
                _dataContext.RemoveRange(curso);


            return base.Atualizar(entity);
        }

    }
}
