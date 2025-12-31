using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface ITratamientoService
    {
        // CRUD de Admin
        Task<PagedResultDto<TratamientoDto>> ObtenerTratamientosAdminAsync(TratamientoFilterDto filtro);
        Task<TratamientoDto> ObtenerTratamientoPorIdAsync(int id);
        Task CrearTratamientoAsync(TratamientoDtoIn dto);
        Task ActualizarTratamientoAsync(int id, TratamientoDtoIn dto);
        Task EliminarTratamientoAsync(int id);
    }
}
