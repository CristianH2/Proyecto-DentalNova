using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IPagoBL
    {
        Task<PagedResultDto<PagoDto>> ObtenerListaPaginadaAsync(PagoFilterDto filtro);
        Task<EstadoCuentaCitaDto> ObtenerEstadoCuentaCitaAsync(int citaId);
        Task<int> RegistrarPagoAsync(PagoDtoIn dto);
    }
}
