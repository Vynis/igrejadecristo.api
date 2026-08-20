using System.ComponentModel.DataAnnotations.Schema;

namespace CursoIgreja.Domain.Models
{
    [Table("pequeno_grupo_relatorio_presencas")]
    public class PequenoGrupoRelatorioPresenca
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("pequeno_grupo_relatorio_id")]
        public int PequenoGrupoRelatorioId { get; set; }
        [Column("pequeno_grupo_membro_id")]
        public int PequenoGrupoMembroId { get; set; }
        [Column("presente")]
        public bool Presente { get; set; }
        [Column("observacao")]
        public string Observacao { get; set; }

        public PequenoGrupoRelatorio PequenoGrupoRelatorio { get; set; }
        public PequenoGrupoMembro PequenoGrupoMembro { get; set; }
    }
}
