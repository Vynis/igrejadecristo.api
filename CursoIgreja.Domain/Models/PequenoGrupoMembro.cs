using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoIgreja.Domain.Models
{
    [Table("pequeno_grupo_membros")]
    public class PequenoGrupoMembro
    {
        public PequenoGrupoMembro()
        {
            DataCadastro = DateTime.Now;
            Status = "A";
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("pequeno_grupo_id")]
        public int PequenoGrupoId { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("data_nascimento")]
        public DateTime? DataNascimento { get; set; }
        [Column("telefone")]
        public string Telefone { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("cep")]
        public string Cep { get; set; }
        [Column("rua_avenida")]
        public string RuaAvenida { get; set; }
        [Column("numero")]
        public string Numero { get; set; }
        [Column("bairro")]
        public string Bairro { get; set; }
        [Column("cidade")]
        public string Cidade { get; set; }
        [Column("estado")]
        public string Estado { get; set; }
        [Column("complemento")]
        public string Complemento { get; set; }
        [Column("tipo")]
        public string Tipo { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("data_entrada")]
        public DateTime? DataEntrada { get; set; }
        [Column("data_saida")]
        public DateTime? DataSaida { get; set; }
        [Column("observacao")]
        public string Observacao { get; set; }
        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; }

        public PequenoGrupo PequenoGrupo { get; set; }
    }
}
