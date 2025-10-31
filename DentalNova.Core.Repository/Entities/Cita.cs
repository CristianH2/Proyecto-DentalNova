using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Repository.Entities
{
    public class Cita
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha y hora de la cita son obligatorias.")]
        [DisplayName("Fecha y Hora")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaHora { get; set; }

        // * RadioButtons *
        [Required(ErrorMessage = "Debe seleccionar una duración.")]
        [DisplayName("Duración (minutos)")]
        public DuracionMinutos DuracionMinutos { get; set; }

        // * Dropdawn list *
        [Required(ErrorMessage = "Debe seleccionar un estatus.")]
        [DisplayName("Estatus de la Cita")]
        public EstatusCita EstatusCita { get; set; }

        // * TextArea *
        [StringLength(255)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Motivo de Consulta")]
        public string? MotivoConsulta { get; set; }

        // * Fecha de creación y actualización *
        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Fecha de Actualización")]
        public DateTime? FechaActualizacion { get; set; }

        // --- Llaves Foráneas (Foreign Keys) ---
        [Required(ErrorMessage = "Debe seleccionar un paciente.")]
        [DisplayName("Paciente")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un odontólogo.")]
        [DisplayName("Odontólogo")]
        public int OdontologoId { get; set; }

        // --- Propiedades de Navegación ---
        [ForeignKey("PacienteId")]
        public virtual Paciente? Paciente { get; set; }

        [ForeignKey("OdontologoId")]
        public virtual Odontologo? Odontologo { get; set; }

        // --- Colecciones (Relaciones de uno a muchos) ---
        public virtual ICollection<CitaTratamiento>? CitasTratamientos { get; set; } = new List<CitaTratamiento>();
        public virtual ICollection<Recordatorio>? Recordatorios { get; set; } = new List<Recordatorio>();

        // --- PROPIEDAD CALCULADA ---
        [NotMapped]
        [DisplayName("Costo Total")]
        public decimal CostoTotalTratamientos => CitasTratamientos?.Sum(ct => ct.CostoFinal) ?? 0;
    }
}
