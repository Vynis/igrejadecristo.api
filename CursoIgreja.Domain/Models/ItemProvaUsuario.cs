using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("itensprovausuario")]
    public class ItemProvaUsuario
    {
        [Key]
        public int Id { get; set; }
        public int ProvaUsuarioId { get; set; }
        public ProvaUsuario ProvaUsuario { get; set; }
        public int ItensProvaId { get; set; }
        public ItemProva ItensProva { get; set; }
        public string Reposta { get; set; }
    }
}
