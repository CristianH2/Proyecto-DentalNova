using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string Token { get; set; } // El JWT generado

        // Para soportar múltiples roles
        public List<string> Roles { get; set; } = new List<string>();

        // Navegacion
        public int? PacienteId { get; set; }
        public int? OdontologoId { get; set; }
    }
}
