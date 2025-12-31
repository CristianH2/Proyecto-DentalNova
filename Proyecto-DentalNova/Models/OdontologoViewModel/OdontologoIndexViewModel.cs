using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;

namespace Proyecto_DentalNova.Models.OdontologoViewModel
{
    public class OdontologoIndexViewModel
    {
        public OdontologoFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<OdontologoDto>? Resultados { get; set; }
    }
}
