using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Dtos
{
    public class AutenticarDto
    {
        public string Email { get; set; }
        public string Senha { get; set; }

        public string Password { get; set; }
    }
}
