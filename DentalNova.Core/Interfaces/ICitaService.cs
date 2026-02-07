using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Interfaces
{
    public interface ICitaService
    {
        Task<PagedResultDto<CitaDto>> ObtenerListaPaginadaAsync(CitaFilterDto filtro);
        Task<CitaDto> ObtenerPorIdAsync(int id);
        Task<int> CrearAsync(CitaDtoIn dto); // Retorna el ID de la cita creada
        Task ActualizarAsync(int id, CitaDtoIn dto);
        Task CambiarEstatusAsync(int id, EstatusCita nuevoEstatus);
        Task EliminarAsync(int id);
    }
}
