using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("logsusuario")]
    public class LogUsuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("data")]
        public DateTime Data { get; set; }
        [Column("acao")]
        public string Acao { get; set; }
        [Column("usuarioid")]
        public int UsuarioId { get; set; }
        public Usuarios Usuario { get; set; }
    }
}
