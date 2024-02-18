using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("usuariosistema")]
    public class UsuarioSistema
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public List<UsuarioPerfis> UsuarioPerfis { get; set; }
    }
}
