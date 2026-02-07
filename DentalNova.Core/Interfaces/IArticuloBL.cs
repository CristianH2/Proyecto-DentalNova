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
        Task<PagedResultDto<ArticuloDto>> ObtenerListaPaginadaAsync(ArticuloFilterDto filtro);
        Task<ArticuloDtoIn> ObtenerParaEditarAsync(int id);
        Task<int> CrearAsync(ArticuloDtoIn dto);
        Task ActualizarAsync(ArticuloDtoIn dto);
        Task EliminarAsync(int id);
        Task CambiarEstatusAsync(int id);
    }
}
