using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class PerfilUsuarioDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string CorreoElectronico { get; set; }
        public string Curp { get; set; }
        public List<string> Roles { get; set; }
    }
}
