using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class CambioPasswordDto
    {
    }

    public class CambioPasswordDtoIn
    {
        [Required]
        public string PasswordActual { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
        public string PasswordNueva { get; set; }
    }
}
