using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("congregacao")]
    public class Congregacao
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("status")]
        public string Status { get; set; }
        public List<Usuarios> Usuarios { get; set; }
        public List<Membro> Membros { get; set; }

    }
}
