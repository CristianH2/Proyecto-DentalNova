using static Proyecto_DentalNova.Models.Enumerables;

namespace Proyecto_DentalNova.Models
{
    public class HorarioOdontologo
    {
        public int Id { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool Activo { get; set; }
        public string Consultorio { get; set; }
        public Odontologo Odontologo { get; set; }
    }
}
