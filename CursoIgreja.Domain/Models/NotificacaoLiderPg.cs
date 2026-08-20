using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoIgreja.Domain.Models
{
    [Table("notificacoes_lider_pg")]
    public class NotificacaoLiderPg
    {
        public NotificacaoLiderPg()
        {
            Status = "A";
            DataCadastro = DateTime.Now;
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("titulo")]
        public string Titulo { get; set; }
        [Column("mensagem")]
        public string Mensagem { get; set; }
        [Column("tipo")]
        public string Tipo { get; set; }
        [Column("link")]
        public string Link { get; set; }
        [Column("data_inicio")]
        public DateTime? DataInicio { get; set; }
        [Column("data_fim")]
        public DateTime? DataFim { get; set; }
        [Column("congregacao_id")]
        public int? CongregacaoId { get; set; }
        [Column("pequeno_grupo_id")]
        public int? PequenoGrupoId { get; set; }
        [Column("lider_pequeno_grupo_id")]
        public int? LiderPequenoGrupoId { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; }
    }
}
