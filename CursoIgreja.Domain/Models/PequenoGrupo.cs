using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoIgreja.Domain.Models
{
    [Table("pequenos_grupos")]
    public class PequenoGrupo
    {
        public PequenoGrupo()
        {
            DataCadastro = DateTime.Now;
            Status = "A";
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("congregacao_id")]
        public int CongregacaoId { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("lider_pequeno_grupo_id")]
        public int LiderPequenoGrupoId { get; set; }
        [Column("co_lider_pequeno_grupo_id")]
        public int? CoLiderPequenoGrupoId { get; set; }
        [Column("dia_semana")]
        public string DiaSemana { get; set; }
        [Column("horario_reuniao")]
        public string HorarioReuniao { get; set; }
        [Column("cep")]
        public string Cep { get; set; }
        [Column("endereco")]
        public string Endereco { get; set; }
        [Column("numero")]
        public string Numero { get; set; }
        [Column("complemento")]
        public string Complemento { get; set; }
        [Column("bairro")]
        public string Bairro { get; set; }
        [Column("cidade")]
        public string Cidade { get; set; }
        [Column("estado")]
        public string Estado { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; }

        public Congregacao Congregacao { get; set; }
        public LiderPequenoGrupo LiderPequenoGrupo { get; set; }
    }
}
