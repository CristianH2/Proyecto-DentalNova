using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class PerfilUsuarioDtoIn
    {
        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(15)]
        public string? Telefono { get; set; }

        public char? Genero { get; set; }
    }
}
