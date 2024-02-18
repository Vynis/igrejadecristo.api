using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("inscricaoliberarcurso")]
    public class InscricaoLiberarCurso
    {
        public int Id { get; set; }
        [Column("ProcessoInscricoesId")]
        public int ProcessoInscricoeId { get; set; }
        public ProcessoInscricao ProcessoInscricoe { get; set; }

        public int CursoId { get; set; }
        public Curso Curso { get; set; }

    }
}
