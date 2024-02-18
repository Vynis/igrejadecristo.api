using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("presencausuario")]
    public class PresencaUsuario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuarios Usuario { get; set; }
        public int ProcessoInscricaoId { get; set; }
        public ProcessoInscricao ProcessoInscricao { get; set; }
        public DateTime DataRegistro { get; set; }
    }
}
