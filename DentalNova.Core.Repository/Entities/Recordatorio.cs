using System.ComponentModel.DataAnnotations.Schema;

namespace DentalNova.Core.Repository.Entities
{
    public class Recordatorio
    {
        public int Id { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Mensaje { get; set; }
        public bool Enviado { get; set; }

        public int CitaId { get; set; } // FK

        [ForeignKey("CitaId")]
        public virtual Cita Cita { get; set; }
    }
}
