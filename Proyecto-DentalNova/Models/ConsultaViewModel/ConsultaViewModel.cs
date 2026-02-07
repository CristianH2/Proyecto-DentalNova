using DentalNova.Core.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DentalNova.Models.ConsultaViewModel
{
    public class ConsultaViewModel
    {
        // Información de Lectura (Contexto)
        public CitaDto Cita { get; set; }
        public PacienteAdminDto Paciente { get; set; }

        // Información de Escritura (Lo que llena el doctor)

        [Required(ErrorMessage = "La nota de evolución es obligatoria.")]
        [Display(Name = "Nota de Evolución / Diagnóstico")]
        public string NotasEvolucion { get; set; }

        [Display(Name = "Recomendaciones / Receta")]
        public string Recomendaciones { get; set; }

        // Para marcar tratamientos como realizados (Checkboxes)
        public List<int> TratamientosRealizadosIds { get; set; } = new List<int>();
    }
}
