using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Repository.Repository.Class
{
    public class CalendarioAulasRepository : RepositoryBase<CalendarioAulas>, ICalendarioAulasRepository
    {
        public CalendarioAulasRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
        }
    }
}
