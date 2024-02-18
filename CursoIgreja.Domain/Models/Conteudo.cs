using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("conteudo")]
    public class Conteudo
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ordem { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public string Arquivo { get; set; }
        public string ArquivoTxt { get; set; }
        public DateTime? DataPeriodoVisualizacaoIni { get; set; }
        public DateTime? DataPeriodoVisualizacaoFim { get; set; }
        public string DefinePeriodoVisualizacao { get; set; }
        public int MinAcerto { get; set; }
        public string LinkConteudoExterno { get; set; }
        public int ModuloId { get; set; }
        public Modulo Modulo { get; set; }

        public List<Anexo> Anexos { get; set; }
        public List<ConteudoUsuario> ConteudoUsuarios { get; set; }
        public List<Prova> Provas { get; set; }


        [NotMapped]
        public bool ConteudoConcluido { get; set; }
    }
}
