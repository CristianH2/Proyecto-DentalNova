using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Repository.Entities
{
    public class Pago
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public Paciente Paciente { get; set; }
        public Cita Cita { get; set; }
    }
}
