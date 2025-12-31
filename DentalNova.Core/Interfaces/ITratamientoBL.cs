using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface ITratamientoBL
    {
        // Existente (Catálogo público)
        Task<IEnumerable<TratamientoDto>> ObtenerCatalogoAsync();

        // --- NUEVOS (Gestión Admin) ---
        Task<PaginatedList<TratamientoDto>> ObtenerListaPaginadaAsync(TratamientoFilterDto filtro);
        Task<TratamientoDto> ObtenerPorIdAdminAsync(int id);
        Task CrearTratamientoAdminAsync(TratamientoDtoIn dto);
        Task ActualizarTratamientoAdminAsync(int id, TratamientoDtoIn dto);
        Task EliminarTratamientoAsync(int id);
    }
}
