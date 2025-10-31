using DentalNova.Core.Repository.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.CitaViewModel
{
    public class CitaVM
    {
        public Cita Cita { get; set; } = new();

        // Listas para los controles del formulario
        public IEnumerable<SelectListItem> PacientesDisponibles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> OdontologosDisponibles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> EstatusDisponibles { get; set; } = new List<SelectListItem>();

        // Esta lista será para los Radio Buttons de Duración
        public IEnumerable<SelectListItem> DuracionesDisponibles { get; set; } = new List<SelectListItem>();
    }
}
