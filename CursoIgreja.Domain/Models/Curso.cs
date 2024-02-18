using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("cursos")]
    public class Curso
    {

        public Curso()
        {
            DataCadastro = DateTime.Now;
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("titulo")]
        public string Titulo { get; set; }
        [Column("datacadastro")]
        public DateTime DataCadastro { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("descricao")]
        public string Descricao { get; set; }
        [Column("cargahoraria")]
        public string CargaHoraria { get; set; }
        [Column("arquivoimg")]
        public string ArquivoImg { get; set; }

        public List<ProcessoInscricao> ProcessoInscricao { get; set; }
        public List<Modulo> Modulo { get; set; }
        public List<CursoProfessor> CursoProfessores { get; set; }
        public List<InscricaoLiberarCurso> InscricaoLiberarCursos { get; set; }
    }
}
