using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("permissoes")]
    public class Permissoes
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Chave { get; set; }
        public int Nivel { get; set; }
        public int IdPai { get; set; }
        public List<PerfilPermissoes> PerfilPermissoes { get; set; }
    }
}
