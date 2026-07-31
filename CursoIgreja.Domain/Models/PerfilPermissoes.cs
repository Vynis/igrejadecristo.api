using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("perfilpermissoes")]
    public class PerfilPermissoes
    {
        public int Id { get; set; }
        public int PefilId { get; set; }
        [ForeignKey("PefilId")]
        public Perfil Perfil { get; set; }
        public int PermissoesId { get; set; }
        [ForeignKey("PermissoesId")]
        public Permissoes Permissoes { get; set; }
    }
}
