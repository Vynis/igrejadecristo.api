using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Repository.Repository.Class
{
    public class TransacaoInscricaoRepository : RepositoryBase<TransacaoInscricao>, ITransacaoInscricaoRepository
    {
        public TransacaoInscricaoRepository(DataContext datacontext, IFiltroDinamico filtroDinamico) : base(datacontext, filtroDinamico)
        {

        }
    }
}
