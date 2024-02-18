using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{

    [Table("perfil")]
    public class Perfil
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public List<UsuarioPerfis> UsuarioPerfis { get; set; }
        public List<PerfilPermissoes> PerfilPermissoes { get; set; }
    }
}
