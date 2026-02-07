using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.ArticuloViewModel
{
    public class ArticuloIndexViewModel
    {
        public ArticuloFilterDto Filtro { get; set; } = new ArticuloFilterDto();
        public PaginatedList<ArticuloDto> Resultados { get; set; }
        public IEnumerable<SelectListItem> Categorias { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> EstatusOpciones { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "true", Text = "Activo" },
            new SelectListItem { Value = "false", Text = "Inactivo" }
        };
    }
}
