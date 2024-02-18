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
    public class InscricaoLiberarCursoRepository : RepositoryBase<InscricaoLiberarCurso>, IInscricaoLiberarCursoRepository
    {
        private readonly DataContext _dataContext;

        public InscricaoLiberarCursoRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<InscricaoLiberarCurso[]> Buscar(Expression<Func<InscricaoLiberarCurso, bool>> predicado)
        {
            IQueryable<InscricaoLiberarCurso> query = _dataContext.InscricaoLiberarCursos.Where(predicado);

            return await query.AsNoTracking().ToArrayAsync();
        }

    }
}
