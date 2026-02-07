using DentalNova.Core.Dtos;

namespace Proyecto_DentalNova.Models.PacienteViewModel
{
    public class PacienteDetalleViewModel
    {
        public PacienteAdminDto Paciente { get; set; }
        public List<CitaDto> HistorialCitas { get; set; } = new List<CitaDto>();

        // Resumen rápido (mostrar en tarjetas)
        public int TotalCitas { get; set; }
        public DateTime? UltimaVisita { get; set; }
        public string ProximoTratamiento { get; set; }
    }
}
