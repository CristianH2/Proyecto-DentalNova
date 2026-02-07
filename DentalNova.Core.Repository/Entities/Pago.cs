using System.ComponentModel.DataAnnotations.Schema;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Repository.Entities
{
    public class Pago
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public MetodoPago MetodoPago { get; set; }

        // FKs Explícitas
        public int PacienteId { get; set; }
        public int CitaId { get; set; }

        [ForeignKey("PacienteId")]
        public virtual Paciente Paciente { get; set; }

        [ForeignKey("CitaId")]
        public virtual Cita Cita { get; set; }
    }
}
