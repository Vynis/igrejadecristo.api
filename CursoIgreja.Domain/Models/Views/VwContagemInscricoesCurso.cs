using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Domain.Models.Views
{
    public class VwContagemInscricoesCurso : BaseView
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Qtd { get; set; }
    }
}
