using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("geolocalizacaousuario")]
    public class GeolocalizacaoUsuario
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime DataRegistro { get; set; }
        public int UsuarioId { get; set; }
        public Usuarios Usuario { get; set; }
    }
}
