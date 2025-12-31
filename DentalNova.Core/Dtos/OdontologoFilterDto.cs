using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class OdontologoFilterDto : PaginacionDto
    {
        public int? Id { get; set; }

        // Filtros de Usuario
        public string? NombreLike { get; set; }
        public string? ApellidosLike { get; set; }
        public string? CorreoLike { get; set; }

        // Filtros de Odontólogo
        public int? EspecialidadId { get; set; }
        public string? CedulaLike { get; set; }
        public DateTime? FechaIngresoDesde { get; set; }
        public DateTime? FechaIngresoHasta { get; set; }
    }
}
