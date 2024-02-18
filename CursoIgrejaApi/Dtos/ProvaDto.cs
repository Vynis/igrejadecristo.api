using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Dtos
{
    public class ProvaDto
    {
        public int Id { get; set; }
        public string Pergunta { get; set; }
        public string TipoComponente { get; set; }
        public string Status { get; set; }
        public int Ordem { get; set; }

        public List<ItemProvaDto> ItensProvas { get; set; }

        //public List<ItemProvaDto> ItensProvas { get { return this.ItensProvas.Where(x => x.Status.Equals("A")).ToList(); } }
    }
}
