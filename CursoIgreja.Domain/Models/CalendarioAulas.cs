using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("calendarioaulas")]
    public class CalendarioAulas
    {
        public int Id { get; set; }
        public DateTime DataAula { get; set; }
        public int ProcessoIncricoesId { get; set; }
        public string Status { get; set; }
        public string Recesso { get; set; }
    }
}
