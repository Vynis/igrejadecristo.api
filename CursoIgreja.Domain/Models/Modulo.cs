using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("modulos")]
    public class Modulo
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ordem { get; set; }
        public int CursoId { get; set; }
        public Curso Curso { get; set; }

        public List<Conteudo> Conteudos { get; set; }
        public List<LiberacaoModulo> LiberacaoModulos { get; set; }
    }
}
