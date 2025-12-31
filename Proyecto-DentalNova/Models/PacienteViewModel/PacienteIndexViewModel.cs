using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;

namespace Proyecto_DentalNova.Models.PacienteViewModel
{
    public class PacienteIndexViewModel
    {
        public PacienteFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<PacienteAdminDto>? Resultados { get; set; }
    }
}
