using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoIgreja.Domain.Models
{
    [Table("pequeno_grupo_relatorios")]
    public class PequenoGrupoRelatorio
    {
        public PequenoGrupoRelatorio()
        {
            DataCadastro = DateTime.Now;
            Status = "Rascunho";
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("pequeno_grupo_id")]
        public int PequenoGrupoId { get; set; }
        [Column("lider_pequeno_grupo_id")]
        public int LiderPequenoGrupoId { get; set; }
        [Column("data_reuniao")]
        public DateTime DataReuniao { get; set; }
        [Column("semana_referencia")]
        public string SemanaReferencia { get; set; }
        [Column("quantidade_ativos")]
        public int QuantidadeAtivos { get; set; }
        [Column("quantidade_rotativos")]
        public int QuantidadeRotativos { get; set; }
        [Column("quantidade_criancas")]
        public int QuantidadeCriancas { get; set; }
        [Column("quantidade_visitantes")]
        public int QuantidadeVisitantes { get; set; }
        [Column("observacao")]
        public string Observacao { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; }
        [Column("data_envio")]
        public DateTime? DataEnvio { get; set; }

        public PequenoGrupo PequenoGrupo { get; set; }
        public LiderPequenoGrupo LiderPequenoGrupo { get; set; }
    }
}
