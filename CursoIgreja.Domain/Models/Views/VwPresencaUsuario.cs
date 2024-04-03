using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models.Views
{
    [Table("vw_presensa_usuario")]
    public class VwPresencaUsuario
    {
        public int Id { get; set; }
        public int ProcessoInscricaoId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataAula { get; set; }
        public string Curso { get; set; }
        public string Status { get; set; }
    }
}
