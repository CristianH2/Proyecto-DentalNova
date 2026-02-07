using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Dtos
{
    public class CitaDto
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime FechaFin { get; set; } // Calculada: FechaHora + Duracion
        public DuracionMinutos DuracionMinutos { get; set; }
        public EstatusCita EstatusCita { get; set; }
        public string EstatusTexto => EstatusCita.ToString();

        // Datos Aplanados del Paciente
        public int PacienteId { get; set; }
        public string PacienteNombre { get; set; }

        // Datos Aplanados del Odontólogo
        public int OdontologoId { get; set; }
        public string OdontologoNombre { get; set; }
        public string OdontologoColor { get; set; } // Opcional: para diferenciar en calendario

        public string MotivoConsulta { get; set; }
        public decimal CostoTotal { get; set; } // Suma de tratamientos

        public List<CitaTratamientoDto> Tratamientos { get; set; } = new List<CitaTratamientoDto>();
    }

    public class CitaDtoIn
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El paciente es obligatorio")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El odontólogo es obligatorio")]
        public int OdontologoId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria")]
        public DuracionMinutos DuracionMinutos { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(255)]
        public string MotivoConsulta { get; set; }

        public EstatusCita EstatusCita { get; set; } = EstatusCita.Programada;

        // Lista opcional de IDs de tratamientos para agregarlos al crear la cita
        public List<int> TratamientosIds { get; set; } = new List<int>();
    }
    public class CitaFilterDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? OdontologoId { get; set; }
        public int? PacienteId { get; set; }
        public EstatusCita? Estatus { get; set; }

        public int Page { get; set; } = 1; // Valor por defecto 1
        public int PageSize { get; set; } = 10;
    }
}
