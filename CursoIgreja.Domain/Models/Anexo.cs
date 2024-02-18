using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("anexos")]
    public class Anexo
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string  Arquivo { get; set; }

        public int ConteudoId { get; set; }
        public Conteudo Conteudo { get; set; }
    }
}
