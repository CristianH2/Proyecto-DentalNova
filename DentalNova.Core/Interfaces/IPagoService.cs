using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IPagoService
    {
        Task<PagedResultDto<PagoDto>> ObtenerListaPaginadaAsync(PagoFilterDto filtro);
        Task<EstadoCuentaCitaDto> ObtenerEstadoCuentaCitaAsync(int citaId);
        Task RegistrarPagoAsync(PagoDtoIn dto);
    }
}
