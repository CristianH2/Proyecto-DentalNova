using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class DashboardDto
    {
        // Tarjetas KPI (Datos del Mes/Día)
        public int CitasHoy { get; set; }
        public int CitasPendientesHoy { get; set; }
        public int PacientesNuevosMes { get; set; }
        public decimal IngresosMes { get; set; }

        // Gráfico Semanal (Últimos 7 días)
        public List<DashboardGraficoDto> DatosGrafico { get; set; } = new List<DashboardGraficoDto>();

        // Tabla: Próximas Citas (Limitado a 5)
        public List<DashboardCitaDto> ProximasCitas { get; set; } = new List<DashboardCitaDto>();
    }

    public class DashboardGraficoDto
    {
        public string DiaSemana { get; set; } // "Lun 10"
        public int CantidadCitas { get; set; }
        public decimal TotalIngresos { get; set; }
    }

    public class DashboardCitaDto
    {
        public int Id { get; set; }
        public string Hora { get; set; }
        public string Paciente { get; set; }
        public string Odontologo { get; set; }
        public string Tratamiento { get; set; } // El primero de la lista o "Consulta"
        public string Estatus { get; set; }
    }
}
