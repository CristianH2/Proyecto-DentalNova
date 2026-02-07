using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;

namespace Proyecto_DentalNova.Models.PagoViewModel
{
    public class PagoIndexViewModel
    {
        public PagoFilterDto Filtro { get; set; } = new PagoFilterDto();
        public PaginatedList<PagoDto> Resultados { get; set; }
    }
}
