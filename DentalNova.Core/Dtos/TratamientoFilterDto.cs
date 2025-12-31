using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class TratamientoFilterDto : PaginacionDto
    {
        public int? Id { get; set; }
        public string? NombreLike { get; set; }
        public decimal? CostoMin { get; set; }
        public decimal? CostoMax { get; set; }
        public int? DuracionMin { get; set; }
        public int? DuracionMax { get; set; }
        public bool? Activo { get; set; }
    }
}
