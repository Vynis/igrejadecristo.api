using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("itensprova")]
    public class ItemProva
    {
        [Key]
        public int Id { get; set; }
        public string Questao { get; set; }
        public string Status { get; set; }
        public string QuestaoCorreta { get; set; }

        public int ProvaId { get; set; }
        public Prova Prova { get; set; }

        public List<ItemProvaUsuario> ItemProvaUsuarios { get; set; }
    }
}
