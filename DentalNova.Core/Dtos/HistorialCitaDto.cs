using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Dtos
{
    public class HistorialCitaDto
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public EstatusCita Estatus { get; set; }
        public string? MotivoConsulta { get; set; }
        public string OdontologoAsignado { get; set; }
        public decimal CostoTotal { get; set; }
        public List<HistorialTratamientoDto> Tratamientos { get; set; }
    }
}
