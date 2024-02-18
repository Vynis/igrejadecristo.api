using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("liberacaomodulo")]
    public class LiberacaoModulo
    {
        public int Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFinal { get; set; }
        public int ModuloId { get; set; }
        public Modulo Modulo { get; set; }
        public int ProcessoInscricoeId { get; set; }
        public ProcessoInscricao ProcessoInscricoe { get; set; }

    }
}
