using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;

namespace Proyecto_DentalNova.Models.OdontologoViewModel
{
    public class OdontologoIndexViewModel
    {
        public OdontologoFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<Odontologo>? Resultados { get; set; }
    }
}
