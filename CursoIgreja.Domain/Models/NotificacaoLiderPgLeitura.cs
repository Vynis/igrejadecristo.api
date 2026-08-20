using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoIgreja.Domain.Models
{
    [Table("notificacoes_lider_pg_leituras")]
    public class NotificacaoLiderPgLeitura
    {
        public NotificacaoLiderPgLeitura()
        {
            DataLeitura = DateTime.Now;
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("notificacao_id")]
        public int NotificacaoId { get; set; }
        [Column("lider_pequeno_grupo_id")]
        public int LiderPequenoGrupoId { get; set; }
        [Column("data_leitura")]
        public DateTime DataLeitura { get; set; }
    }
}
