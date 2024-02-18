using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("professores")]
    public class Professor
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Foto { get; set; }
        public string Status { get; set; }

        public List<CursoProfessor> CursoProfessores { get; set; }
    }
}
