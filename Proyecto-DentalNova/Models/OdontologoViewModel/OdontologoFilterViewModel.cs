using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DentalNova.Models.OdontologoViewModel
{
    public class OdontologoFilterViewModel
    {
        // --- Filtros ---
        [Display(Name = "ID")]
        public int? Id { get; set; }

        [Display(Name = "Nombre")]
        public string? NombreLike { get; set; }

        [Display(Name = "Apellidos")]
        public string? ApellidosLike { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string? CorreoLike { get; set; }

        [Display(Name = "Especialidad")]
        public int? EspecialidadId { get; set; }

        [Display(Name = "Cédula Profesional")]
        public string? CedulaLike { get; set; }

        [Display(Name = "Ingreso Desde")]
        public DateTime? FechaIngresoDesde { get; set; }

        [Display(Name = "Ingreso Hasta")]
        public DateTime? FechaIngresoHasta { get; set; }

        // --- Listas para DropDowns ---
        public IEnumerable<SelectListItem> EspecialidadesDisponibles { get; set; } = new List<SelectListItem>();

        // --- Paginación ---
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
