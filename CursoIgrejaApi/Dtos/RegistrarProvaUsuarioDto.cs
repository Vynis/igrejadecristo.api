using CursoIgreja.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Dtos
{
    public class RegistrarProvaUsuarioDto
    {
        public List<ProvaUsuario> ProvaUsuario { get; set; }
    }
}
