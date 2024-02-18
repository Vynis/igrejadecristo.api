using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Dtos
{
    public class LiberacaoModuloDto
    {
        public int Id { get; set; }
        public DateTime DataInicio { get; set; }
        public int ModuloId { get; set; }
        public int ProcessoInscricoeId { get; set; }
    }
}
