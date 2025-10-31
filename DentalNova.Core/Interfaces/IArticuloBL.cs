using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IArticuloBL
    {
        Task<IEnumerable<ArticuloDto>> ObtenerCatalogoAsync();
    }
}
