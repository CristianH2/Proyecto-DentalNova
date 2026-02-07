using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class LoginResponseDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
        public int? PacienteId { get; set; }
        public int? OdontologoId { get; set; }
    }
}
