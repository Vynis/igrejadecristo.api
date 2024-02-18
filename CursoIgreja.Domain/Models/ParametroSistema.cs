using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("parametrosistema")]
    public class ParametroSistema
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("titulo")]
        public string Titulo { get; set; }
        [Column("valor")]
        public string Valor { get; set; }
        [Column("status")]
        public string Status { get; set; }
    }
}
