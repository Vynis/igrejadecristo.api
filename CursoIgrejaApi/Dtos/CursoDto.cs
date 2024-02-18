using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Dtos
{
    public class CursoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Status { get; set; }
        public string Descricao { get; set; }
        public string CargaHoraria { get; set; }
        public string ArquivoImg { get; set; }
    }
}
