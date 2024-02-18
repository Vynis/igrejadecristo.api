using FiltrDinamico.Core.Models;
using System.Collections.Generic;

namespace CursoIgreja.Domain.Models
{
    public class PaginationFilter
    {
        public IEnumerable<FiltroItem> Filtro { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}
