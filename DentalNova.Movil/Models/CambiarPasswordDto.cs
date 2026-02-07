using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class CambiarPasswordDto
    {
        public string PasswordActual { get; set; }
        public string PasswordNuevo { get; set; }
    }
}
