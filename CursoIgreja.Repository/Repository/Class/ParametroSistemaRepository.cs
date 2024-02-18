﻿using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Repository.Repository.Class
{
    public class ParametroSistemaRepository : RepositoryBase<ParametroSistema>, IParametroSistemaRepository
    {
        public ParametroSistemaRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {

        }
    }
}