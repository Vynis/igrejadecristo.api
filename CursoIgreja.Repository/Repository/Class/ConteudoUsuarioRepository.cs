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
    public class ConteudoUsuarioRepository : RepositoryBase<ConteudoUsuario>, IConteudoUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public ConteudoUsuarioRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<ConteudoUsuario[]> Buscar(Expression<Func<ConteudoUsuario, bool>> predicado)
        {
            IQueryable<ConteudoUsuario> query = _dataContext.ConteudoUsuarios
                                                                .Where(predicado)
                                                                .Include(c => c.Conteudo)
                                                                .Include(c => c.Conteudo.Anexos)
                                                                .Include(c => c.Usuarios)
                                                                .Include(c => c.Conteudo.Modulo)
                                                                .Include(c => c.Conteudo.Modulo.Curso)
                                                                .Include(c => c.Conteudo.Provas)
                                                                .Include("Conteudo.Provas.ItensProvas")
                                                                .Include("Conteudo.Provas.ProvaUsuarios");

            return await query.AsNoTracking().ToArrayAsync();
        }

        public async override Task<ConteudoUsuario[]> ObterTodos()
        {

            IQueryable<ConteudoUsuario> query = _dataContext.ConteudoUsuarios
                                                                .Include(c => c.Conteudo)
                                                                .Include(c => c.Usuarios);

            return await query.AsNoTracking().ToArrayAsync();
        }

    }
}
