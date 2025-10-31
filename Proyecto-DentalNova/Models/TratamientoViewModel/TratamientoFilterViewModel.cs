using System.ComponentModel.DataAnnotations;

namespace Proyecto_DentalNova.Models.TratamientoViewModel
{
    public class TratamientoFilterViewModel
    {
        [Display(Name = "ID del Tratamiento")]
        public int? Id { get; set; }

        [Display(Name = "Nombre del Tratamiento")]
        public string? NombreLike { get; set; }

        [Display(Name = "Costo Mínimo")]
        public decimal? CostoMin { get; set; }

        [Display(Name = "Costo Máximo")]
        public decimal? CostoMax { get; set; }

        [Display(Name = "Duración Mínima (días)")]
        public int? DuracionMin { get; set; }

        [Display(Name = "Duración Máxima (días)")]
        public int? DuracionMax { get; set; }

        [Display(Name = "Estado")]
        public bool? Activo { get; set; }

        // --- Paginación ---
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
