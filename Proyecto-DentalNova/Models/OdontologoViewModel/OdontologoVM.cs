using DentalNova.Core.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.OdontologoViewModel
{
    public class OdontologoVM
    {
        // Contiene la instancia del Odontólogo que se está creando o editando.
        public OdontologoDtoIn Odontologo { get; set; } = new();

        // Lista para poblar el DropDownList de Usuarios.
        // Se llenará solo con usuarios activos que no sean ni pacientes ni otros odontólogos.
        public IEnumerable<SelectListItem> UsuariosDisponibles { get; set; } = new List<SelectListItem>();

        // Propiedad para POBLAR el <select>.
        public IEnumerable<SelectListItem> TodasLasEspecialidades { get; set; } = new List<SelectListItem>();
        
        // Propiedad para recibir los IDs seleccionados del formulario.
        public int[]? EspecialidadesSeleccionadasIds { get; set; }
    }
}
