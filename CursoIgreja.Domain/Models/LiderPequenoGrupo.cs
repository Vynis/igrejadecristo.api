using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoIgreja.Domain.Models
{
    [Table("lideres_pequeno_grupo")]
    public class LiderPequenoGrupo
    {
        public LiderPequenoGrupo()
        {
            DataInicio = DateTime.Now;
            DataCadastro = DateTime.Now;
            Status = "A";
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        [Column("congregacao_id")]
        public int? CongregacaoId { get; set; }
        [Column("data_inicio")]
        public DateTime DataInicio { get; set; }
        [Column("data_fim")]
        public DateTime? DataFim { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("observacao")]
        public string Observacao { get; set; }
        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; }

        public Usuarios Usuario { get; set; }
        public Congregacao Congregacao { get; set; }
    }
}
