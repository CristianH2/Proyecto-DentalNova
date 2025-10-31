namespace DentalNova.Core.Repository.Entities
{
    public class Recordatorio
    {
        public int Id { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string Mensaje { get; set; }
        public bool Enviado { get; set; }
        public Cita Cita { get; set; }
    }
}
