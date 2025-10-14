using System.ComponentModel;
using static Proyecto_DentalNova.Models.Enumerables;

namespace Proyecto_DentalNova.Models
{
    public class CitaTratamiento
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Observaciones")]
        public string? Observaciones { get; set; }

        [DisplayName("Costo Final")]
        public decimal CostoFinal { get; set; }

        [DisplayName("Estatus del Tratamiento")]
        public EstatusTratamiento EstatusTratamiento { get; set; }

        // --- Llaves Foráneas (Foreign Keys) ---
        [DisplayName("Cita")]
        public int CitaId { get; set; }

        [DisplayName("Tratamiento")]
        public int TratamientoId { get; set; }

        // --- Propiedades de Navegación ---
        public virtual Cita? Cita { get; set; }
        public virtual Tratamiento? Tratamiento { get; set; }
    }
}
