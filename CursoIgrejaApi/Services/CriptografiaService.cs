using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Services
{
    public class CriptografiaService
    {
        public static string Criptografar(string valor)
        {
            string chaveCripto;
            Byte[] cript = System.Text.ASCIIEncoding.ASCII.GetBytes(valor);
            chaveCripto = Convert.ToBase64String(cript);
            return chaveCripto;
        }

        public static string Descriptografar(string valor)
        {
            string chaveCripto;
            Byte[] cript = Convert.FromBase64String(valor);
            chaveCripto = System.Text.ASCIIEncoding.ASCII.GetString(cript);
            return chaveCripto;
        }


    }
}
