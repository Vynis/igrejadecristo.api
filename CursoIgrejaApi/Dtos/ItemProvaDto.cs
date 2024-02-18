using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Dtos
{
    public class ItemProvaDto
    {
        public int Id { get; set; }
        public string Questao { get; set; }
        public string Status { get; set; }
        public string QuestaoCorreta { get; set; }

        public int ProvaId { get; set; }
    }
}
