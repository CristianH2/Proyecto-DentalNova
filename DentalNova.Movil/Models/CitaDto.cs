using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class CitaDto
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string EstatusTexto { get; set; } // Ej: "Confirmada"
        public string OdontologoNombre { get; set; }
        public string MotivoConsulta { get; set; }
        public decimal CostoTotal { get; set; }

        // Propiedad auxiliar para la vista
        public string FechaFormateada => FechaHora.ToString("dd MMM yyyy - hh:mm tt");
    }

    // La respuesta envoltorio
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
