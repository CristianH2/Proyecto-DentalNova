using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Dtos
{
    // --- DTO de Salida (Para ver datos) ---
    public class HorarioOdontologoDto
    {
        public int Id { get; set; }
        public int OdontologoId { get; set; }
        public string OdontologoNombre { get; set; } // Relación aplanada
        public DiaSemana DiaSemana { get; set; }
        public string DiaSemanaTexto => DiaSemana.ToString(); // Helper visual
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Consultorio { get; set; }
        public bool Activo { get; set; }

        // Formato para el frontend (ej. "09:00 - 13:00")
        public string RangoHorario => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
    }

    public class HorarioOdontologoDtoIn
    {
        public int Id { get; set; } // 0 en Create, >0 en Update

        [Required(ErrorMessage = "El odontólogo es requerido")]
        public int OdontologoId { get; set; }

        [Required(ErrorMessage = "El día de la semana es requerido")]
        [Range(1, 7, ErrorMessage = "Día inválido")]
        public DiaSemana DiaSemana { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es requerida")]
        public TimeSpan HoraFin { get; set; }

        [Required(ErrorMessage = "El consultorio es requerido")]
        [StringLength(50, ErrorMessage = "El consultorio no puede exceder 50 caracteres")]
        public string Consultorio { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class HorarioOdontologoFilterDto
    {
        public int? OdontologoId { get; set; }
        public DiaSemana? DiaSemana { get; set; }
        public bool? SoloActivos { get; set; } = true;
    }
}
