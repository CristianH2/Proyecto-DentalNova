using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Dtos
{
    public class HistorialTratamientoDto
    {
        public string NombreTratamiento { get; set; }
        public EstatusTratamiento Estatus { get; set; }
        public decimal CostoFinal { get; set; }
        public string? Observaciones { get; set; }
    }
}
