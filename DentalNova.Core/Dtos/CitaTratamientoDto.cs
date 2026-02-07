using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class CitaTratamientoDto
    {
        public int Id { get; set; }
        public int TratamientoId { get; set; }
        public string TratamientoNombre { get; set; }
        public decimal CostoFinal { get; set; } // El precio congelado
        public string? Observaciones { get; set; }
    }
}
