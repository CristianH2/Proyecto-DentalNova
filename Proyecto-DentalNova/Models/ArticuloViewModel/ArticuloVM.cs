using DentalNova.Core.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.ArticuloViewModel
{
    public class ArticuloVM
    {
        public ArticuloDtoIn Articulo { get; set; } = new ArticuloDtoIn();
        public IEnumerable<SelectListItem> Categorias { get; set; } = new List<SelectListItem>(); // Dropdown de Categoría
    }
}
