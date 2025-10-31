using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;

namespace Proyecto_DentalNova.Models.CitaViewModel
{
    public class CitaIndexViewModel
    {
        public CitaFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<Cita>? Resultados { get; set; }
    }
}
