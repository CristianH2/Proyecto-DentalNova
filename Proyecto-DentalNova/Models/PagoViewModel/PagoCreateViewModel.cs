using DentalNova.Core.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.PagoViewModel
{
    public class PagoCreateViewModel
    {
        public PagoDtoIn Pago { get; set; } = new PagoDtoIn();

        [ValidateNever]
        public EstadoCuentaCitaDto EstadoCuenta { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> MetodosPago { get; set; }
    }
}
