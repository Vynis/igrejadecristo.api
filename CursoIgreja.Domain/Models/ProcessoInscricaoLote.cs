using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CursoIgreja.Domain.Models
{
    [Table("processoinscricaolotes")]
    public class ProcessoInscricaoLote
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("processoinscricaoid")]
        public int ProcessoInscricaoId { get; set; }
        [JsonIgnore]
        public ProcessoInscricao ProcessoInscricao { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("datainicial")]
        public DateTime DataInicial { get; set; }

        [Column("datafinal")]
        public DateTime DataFinal { get; set; }

        [Column("valor")]
        public decimal Valor { get; set; }

        [Column("valorpixboleto")]
        public decimal? ValorPixBoleto { get; set; }

        [Column("status")]
        public string Status { get; set; }
    }
}
