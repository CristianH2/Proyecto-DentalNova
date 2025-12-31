using DentalNova.Core.Dtos;

namespace Proyecto_DentalNova.Models.PacienteViewModel
{
    //public record UsuarioDisponible(int Id, string Texto, DateTime? FechaNacimiento);
    public class PacienteVM
    {
        // Usamos el DTO de Entrada en lugar de la Entidad
        public PacienteAdminDtoIn Paciente { get; set; } = new();

        // 2. Usamos el DTO de usuarios disponibles que viene de la API
        public List<UsuarioDisponibleDto> UsuariosDisponibles { get; set; } = new List<UsuarioDisponibleDto>();
    }
}
