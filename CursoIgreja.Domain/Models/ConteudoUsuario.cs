using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("conteudousuarios")]
    public class ConteudoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Concluido { get; set; }
        public DateTime? DataConclusao { get; set; }

        public int ConteudoId { get; set; }
        public Conteudo Conteudo { get; set; }
       
        public int UsuariosId { get; set; }
        public Usuarios Usuarios { get; set; }
    }
}
