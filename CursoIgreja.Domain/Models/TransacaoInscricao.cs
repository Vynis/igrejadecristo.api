using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("transacaoinscricao")]
    public class TransacaoInscricao
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("codigo")]
        public string Codigo { get; set; }

        [Column("inscricaousuarioid")]
        public int InscricaoUsuarioId { get; set; }
        public InscricaoUsuario InscricaoUsuario { get; set; }

    }
}
