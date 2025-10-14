using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Proyecto_DentalNova.Models.Enumerables;

namespace Proyecto_DentalNova.Models
{
    public class Cita
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        // * Fechas y Horas *
        [DisplayName("Fecha y Hora")]
        public DateTime FechaHora { get; set; }

        // * RadioButtons *
        [DisplayName("Duración (minutos)")]
        public DuracionMinutos DuracionMinutos { get; set; }

        // * Dropdawn list *
        [DisplayName("Estatus de la Cita")]
        public EstatusCita EstatusCita { get; set; }

        // * TextArea *
        [DisplayName("Motivo de Consulta")]
        public string? MotivoConsulta { get; set; }

        // * Fecha de creación y actualización *
        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Fecha de Actualización")]
        public DateTime? FechaActualizacion { get; set; }

        // --- Llaves Foráneas (Foreign Keys) ---
        [DisplayName("Paciente")]
        public int PacienteId { get; set; }
        [DisplayName("Odontólogo")]
        public int OdontologoId { get; set; }

        // --- Propiedades de Navegación ---
        public virtual Paciente? Paciente { get; set; }
        public virtual Odontologo? Odontologo { get; set; }

        // --- Colecciones (Relaciones de uno a muchos) ---
        public virtual ICollection<CitaTratamiento>? CitasTratamientos { get; set; } = new List<CitaTratamiento>();
        public virtual ICollection<Recordatorio>? Recordatorios { get; set; } = new List<Recordatorio>();
    }
}
