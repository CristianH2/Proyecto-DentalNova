using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class NotificacionDto
    {
        public int Id { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Mensaje { get; set; }
        public string DoctorNombre { get; set; }

        public string FechaFormateada => FechaEnvio.ToString("dd MMM - hh:mm tt");
    }
}
