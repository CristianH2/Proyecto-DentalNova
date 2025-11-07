using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class CitaAgendadaDto
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estatus { get; set; }
        public string MotivoConsulta { get; set; }
        public string OdontologoAsignado { get; set; }
        public string TratamientoInicial { get; set; }
    }
}
