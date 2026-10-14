using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoIgreja.Domain.Models
{
    [Table("recuperacaosenha")]
    public class RecuperacaoSenha
    {
        public RecuperacaoSenha()
        {
            DataCadastro = DateTime.Now;
            Status = "P";
            Tentativas = 0;
        }

        [Column("id")]
        public int Id { get; set; }

        [Column("usuarioid")]
        public int UsuarioId { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; }

        [Column("tokentemporario")]
        public string TokenTemporario { get; set; }

        [Column("dataexpiracao")]
        public DateTime DataExpiracao { get; set; }

        [Column("tentativas")]
        public int Tentativas { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("datacadastro")]
        public DateTime DataCadastro { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuarios Usuario { get; set; }
    }
}
