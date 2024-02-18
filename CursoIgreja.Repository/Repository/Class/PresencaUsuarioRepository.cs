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
    public class PresencaUsuarioRepository : RepositoryBase<PresencaUsuario>, IPresencaUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public PresencaUsuarioRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<PresencaUsuario[]> Buscar(Expression<Func<PresencaUsuario, bool>> predicado)
        {
            IQueryable<PresencaUsuario> query = _dataContext.PresencaUsuarios
                                                    .Where(predicado)
                                                    .Include(c => c.Usuario)
                                                    .Include(c => c.ProcessoInscricao)
                                                    .Include(c => c.ProcessoInscricao.Curso);

            return await query.AsNoTracking().ToArrayAsync();

        }
    }
}
