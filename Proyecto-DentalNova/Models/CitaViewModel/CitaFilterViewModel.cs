using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace Proyecto_DentalNova.Models.CitaViewModel
{
    public class CitaFilterViewModel
    {
        [Display(Name = "ID de Cita")]
        public int? Id { get; set; }

        // CAMBIAMOS LOS IDs POR CAMPOS DE TEXTO
        [Display(Name = "Paciente")]
        public string? PacienteNombreLike { get; set; }

        [Display(Name = "Odontólogo")]
        public string? OdontologoNombreLike { get; set; }

        [Display(Name = "Desde")]
        public DateTime? FechaDesde { get; set; }

        [Display(Name = "Hasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(Name = "Estatus de la Cita")]
        public EstatusCita? Estatus { get; set; }

        public IEnumerable<SelectListItem> EstatusDisponibles { get; set; } = new List<SelectListItem>();

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
