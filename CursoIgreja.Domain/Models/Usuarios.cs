using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("usuarios")]
    public class Usuarios
    {
        public Usuarios()
        {
            Status = "A";
            DataCadastro = DateTime.Now;
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("senha")]
        public string Senha { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("datacadastro")]
        public DateTime DataCadastro { get; set; }
        [Column("cpf")]
        public string Cpf { get; set; }
        [Column("datanascimento")]
        public DateTime DataNascimento { get; set; }
        [Column("tipoacesso")]
        public string TipoAcesso { get; set; }

        [Column("rua")]
        public string Rua { get; set; }
        [Column("complemento")]
        public string Complemento { get; set; }
        [Column("bairro")]
        public string Bairro { get; set; }
        [Column("cidade")]
        public string Cidade { get; set; }
        [Column("estado")]
        public string Estado { get; set; }
        [Column("numero")]
        public string Numero { get; set; }
        [Column("cep")]
        public string Cep { get; set; }
        [Column("telefonecelular")]
        public string TelefoneCelular { get; set; }
        [Column("telefonefixo")]
        public string TelefoneFixo { get; set; }

        [Column("congregahaquantotempo")]
        public string CongregaHaQuantoTempo { get; set; }
        [Column("recebepastoreiro")]
        public string RecebePastoreiro { get; set; }
        [Column("quempastoreia")]
        public string QuemPastoreia { get; set; }
        [Column("frequentacelula")]
        public string FrequentaCelula { get; set; }
        [Column("quemlider")]
        public string QuemLider { get; set; }

        [Column("congregacaoid")]
        public int CongregacaoId { get; set; }
        public Congregacao Congregacao { get; set; }

        public List<InscricaoUsuario> InscricaoUsuarios { get; set; }

        public List<LogUsuario> logUsuario { get; set; }
        public List<ConteudoUsuario> ConteudoUsuarios { get; set; }

        public List<ProvaUsuario> ProvaUsuarios { get; set; }
        public List<GeolocalizacaoUsuario> GeolocalizacaoUsuarios { get; set; }
        public List<PresencaUsuario> PresencaUsuarios { get; set; }

    }
}

