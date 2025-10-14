using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.PacienteViewModel
{
    public record UsuarioDisponible(int Id, string Texto, DateTime? FechaNacimiento);
    public class PacienteVM
    {
        // 1. Contiene la instancia del Paciente que se está creando o editando.
        public Paciente Paciente { get; set; } = new();

        // 2. Lista para poblar el DropDownList de Usuarios.
        //    Esta lista se llenará en el controlador solo con los usuarios
        //    que todavía no están asignados a otro paciente.
        public IEnumerable<UsuarioDisponible> UsuariosDisponibles { get; set; } = new List<UsuarioDisponible>();
    }
}
