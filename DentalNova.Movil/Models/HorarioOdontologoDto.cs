using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class HorarioOdontologoDto
    {
        public int Id { get; set; }
        public int OdontologoId { get; set; }
        public int DiaSemana { get; set; } // 1=Lunes ... 7=Domingo
        public string HoraInicio { get; set; } // Formato "HH:mm:ss" o "HH:mm"
        public string HoraFin { get; set; }
        public bool Activo { get; set; }
    }
}
