using CursoIgreja.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Dtos
{
    public class ConteudoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ordem { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public string Arquivo { get; set; }
        public string ArquivoTxt { get; set; }
        public int MinAcerto { get; set; }
        public string LinkConteudoExterno { get; set; }
        public int ModuloId { get; set; }
        public ModuloDto Modulo { get; set; }
        public bool ConteudoConcluido { get; set; }
        public List<ProvaDto> Provas { get; set; }
    }
}
