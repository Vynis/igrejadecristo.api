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
    public class UsuariosRepository : RepositoryBase<Usuarios>, IUsuariosRepository
    {
        private readonly DataContext _dataContext;

        public UsuariosRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<Usuarios> ObterPorId(int id)
        {
            IQueryable<Usuarios> query = _dataContext.Usuarios;

            return await query.AsNoTracking().Where(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}
