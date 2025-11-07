using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class CitaDto
    {
    }

    public class CitaDtoIn
    {
        [Required]
        public DateTime FechaHora { get; set; }

        [StringLength(255)]
        public string? MotivoConsulta { get; set; }
    }
}
