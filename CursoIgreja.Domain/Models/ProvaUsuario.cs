using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("provausuario")]
    public class ProvaUsuario
    {
        public ProvaUsuario()
        {
            DataCadastro = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }
        public string PerguntaTexto { get; set; }
        public DateTime DataCadastro { get; set; }
        public int UsuarioId { get; set; }
        public Usuarios Usuario { get; set; }
        public int ProvaId { get; set; }
        public Prova Prova { get; set; }


        public List<ItemProvaUsuario> ItemProvaUsuarios { get; set; }

    }
}
