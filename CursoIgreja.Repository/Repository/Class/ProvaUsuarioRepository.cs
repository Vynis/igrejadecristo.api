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
    public class ProvaUsuarioRepository: RepositoryBase<ProvaUsuario>, IProvaUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public ProvaUsuarioRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<ProvaUsuario[]> Buscar(Expression<Func<ProvaUsuario, bool>> predicado)
        {
            IQueryable<ProvaUsuario> query = _dataContext.ProvaUsuarios
                                                                .Where(predicado)
                                                                .Include(c => c.Usuario)
                                                                .Include(c => c.Prova)
                                                                .Include(c => c.Prova.Conteudo);

            return await query.AsNoTracking().ToArrayAsync();
        }
    }
}
