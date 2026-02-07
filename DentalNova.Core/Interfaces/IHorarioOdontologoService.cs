using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IHorarioOdontologoService
    {
        Task<List<HorarioOdontologoDto>> ObtenerPorOdontologoAsync(int odontologoId);
        Task<HorarioOdontologoDto> ObtenerPorIdAsync(int id);
        Task CrearAsync(HorarioOdontologoDtoIn dto);
        Task ActualizarAsync(int id, HorarioOdontologoDtoIn dto);
        Task EliminarAsync(int id);
    }
}
