using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models.Views
{
    public class VwRelatorioInscricoes
    {
        public int IdUsuario { get; set; }
        public int IdInscricao { get; set; }
        public string Nome { get; set; }
        public string TelefoneCelular { get; set; }
        public string TelefoneFixo { get; set; }
        public string Curso { get; set; }
        public string Status { get; set; }
        public DateTime DataInscricao { get; set; }
        public string Ciclo { get; set; }
        public string Ano { get; set; }
        public string MeioPagamento { get; set; }
        public string MeioPagamentoDesc { get; set; }
        public string ValorLiquido { get; set; }
        public string ValorBruto { get; set; }
        public string QtdParcelas { get; set; }
    }
}
