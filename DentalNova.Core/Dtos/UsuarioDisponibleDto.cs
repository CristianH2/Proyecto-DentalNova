using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class UsuarioDisponibleDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        // Propiedad auxiliar para mostrar en el dropdown
        public string TextoMostrar => $"{NombreCompleto} ({Correo})";
    }
}
