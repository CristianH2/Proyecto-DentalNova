namespace DentalNova.Core.Repository.Entities
{
    public class LogActividad
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string AccionRealizada { get; set; }
        public string Detalles { get; set; }
        public Usuario Usuario { get; set; }
    }
}
