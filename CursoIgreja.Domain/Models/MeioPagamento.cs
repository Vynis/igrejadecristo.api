using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("meiospagamentos")]
    public class MeioPagamento
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("titulo")]
        public string Titulo { get; set; }
        [Column("token")]
        public string Token { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("status")]
        public string Status { get; set; }
    }
}
