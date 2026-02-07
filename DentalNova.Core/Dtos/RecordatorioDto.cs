using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class RecordatorioDto
    {
        public int Id { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Mensaje { get; set; }
        public bool Enviado { get; set; }

        // --- Datos de Referencia ---
        public int CitaId { get; set; }
        public DateTime FechaCita { get; set; }
        public string DoctorNombre { get; set; }
        public string PacienteNombre { get; set; }
    }

    public class RecordatorioDtoIn
    {
        [Required(ErrorMessage = "El ID de la cita es obligatorio.")]
        public int CitaId { get; set; }

        [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres.")]
        public string? MensajePersonalizado { get; set; }
    }

    public class RecordatorioFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int? PacienteId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool? Enviado { get; set; }
    }


}
