using DentalNova.Core.Dtos;
using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface ICitaBL
    {
        Task<PagedResultDto<CitaDto>> ObtenerListaPaginadaAsync(CitaFilterDto filtro, int page, int pageSize);
        Task<CitaDto> ObtenerPorIdAsync(int id);
        Task<int> CrearAsync(CitaDtoIn dto);
        Task ActualizarAsync(CitaDtoIn dto);
        Task CambiarEstatusAsync(int id, Enumerables.EstatusCita nuevoEstatus);
        Task EliminarAsync(int id);
    }
}
