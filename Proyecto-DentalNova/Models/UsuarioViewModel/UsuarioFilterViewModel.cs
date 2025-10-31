using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DentalNova.Models.UsuarioViewModel
{
    public class UsuarioFilterViewModel
    {
        // --- Campos del Filtro ---
        [Display(Name = "ID de Usuario")]
        public int? Id { get; set; }

        [Display(Name = "Nombre")]
        public string? NombreLike { get; set; }

        [Display(Name = "Apellidos")]
        public string? ApellidosLike { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string? CorreoLike { get; set; }

        [Display(Name = "Teléfono")]
        public string? TelefonoLike { get; set; }

        [Display(Name = "Género")]
        public char? Genero { get; set; }

        [Display(Name = "Estado")]
        public bool? Activo { get; set; }

        // --- Listas para los DropDowns del filtro ---
        public IEnumerable<SelectListItem> Generos { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Estados { get; set; } = new List<SelectListItem>();

        // --- Paginación ---
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
