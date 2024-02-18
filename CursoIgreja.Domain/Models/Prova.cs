using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("provas")]
    public class Prova
    {
        [Key]
        public int Id { get; set; }
        public string Pergunta { get; set; }
        public string TipoComponente { get; set; }
        public string Status { get; set; }
        public int Ordem { get; set; }

        public int ConteudoId { get; set; }
        public Conteudo Conteudo { get; set; }

        public List<ItemProva> ItensProvas { get; set; }
        public List<ProvaUsuario> ProvaUsuarios { get; set; }
    }
}
