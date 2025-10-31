using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;

namespace Proyecto_DentalNova.Models.TratamientoViewModel
{
    public class TratamientoIndexViewModel
    {
        public TratamientoFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<Tratamiento>? Resultados { get; set; }
    }
}
