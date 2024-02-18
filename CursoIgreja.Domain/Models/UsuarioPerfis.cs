using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("usuarioperfis")]
    public class UsuarioPerfis
    {
        public int Id { get; set; }
        public int PerfilId { get; set; }
        public Perfil Perfil { get; set; }
        public int UsuarioSistemaId { get; set; }
        public UsuarioSistema UsuarioSistema { get; set; }
    }
}
