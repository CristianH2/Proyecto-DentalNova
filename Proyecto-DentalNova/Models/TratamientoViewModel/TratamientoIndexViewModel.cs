using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;

namespace Proyecto_DentalNova.Models.TratamientoViewModel
{
    public class TratamientoIndexViewModel
    {
        public TratamientoFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<TratamientoDto>? Resultados { get; set; }
    }
}
