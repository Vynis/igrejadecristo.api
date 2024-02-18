using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("membros")]
    public class Membro
    {

        public Membro()
        {
            DataCadastro = DateTime.Today;
            Status = "A";
        }

        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; }
        public string TelefoneFixo { get; set; }
        public string TelefoneCelular { get; set; }
        public string EstadoCivil { get; set; }
        public string Email { get; set; }
        public string Cep { get; set; }
        public string RuaAvenida { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Complemento { get; set; }
        public int CongregacaoId { get; set; }
        public Congregacao Congregacao { get; set; }
        public DateTime DataInsercaoIgreja { get; set; }
        public int ComoChegouIgreja { get; set; }
        public DateTime DataConversao { get; set; }
        public string EhBatizado { get; set; }
        public DateTime? DataBatismo { get; set; }
        public string FazParteGrupoPequeno { get; set; }
        public string QualGrupoPequeno { get; set; }
        public string FazParteMinisterio { get; set; }
        public string QualMinisterio { get; set; }
        public string DeQuemRecebeuPastoreiro { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Status { get; set; }
        public bool PossuiEmail { get; set; }
    }
}
