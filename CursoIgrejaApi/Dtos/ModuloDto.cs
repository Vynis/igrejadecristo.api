using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Dtos
{
    public class ModuloDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ordem { get; set; }
        public int CursoId { get; set; }
        public CursoDto Curso { get; set; }
        public List<ConteudoDto> Conteudos { get; set; }
        public List<LiberacaoModuloDto> LiberacaoModulos { get; set; }
    }
}
