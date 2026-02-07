using DentalNova.Core.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.OdontologoViewModel
{
    public class OdontologoVM
    {
        // --- Para Create / Edit Odontólogo ---
        public OdontologoDtoIn Odontologo { get; set; } = new();
        public IEnumerable<SelectListItem> UsuariosDisponibles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TodasLasEspecialidades { get; set; } = new List<SelectListItem>();
        public int[]? EspecialidadesSeleccionadasIds { get; set; }

        // --- Para la Gestión de Horarios ---
        public OdontologoDto? OdontologoVisual { get; set; }
        public List<HorarioOdontologoDto> Horarios { get; set; } = new();

        // Para el formulario de CREAR
        public HorarioOdontologoDtoIn NuevoHorario { get; set; } = new();

        // Para el formulario de EDITAR (Nuevo)
        public HorarioOdontologoDtoIn HorarioEdicion { get; set; } = new();
    }
}
