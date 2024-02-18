using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Domain.Models.Views
{
    public class VwRelatorioPresencaAula
    {
        public string Nome { get; set; }
        public int ProcessoInscricaoId { get; set; }
        public DateTime? DataRegistro { get; set; }
    }
}
