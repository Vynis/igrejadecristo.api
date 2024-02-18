using CursoIgreja.Domain.Models.Views;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Repository.Repository.Class
{
    public class VwContagemInscricaoCursoRepository : RepositoryViewBase<VwContagemInscricoesCurso>, IVwContagemInscricaoCursoRepository
    {
        public VwContagemInscricaoCursoRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {

        }
    }
}
