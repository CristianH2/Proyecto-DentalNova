using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IHorarioOdontologoBL
    {
        Task<HorarioOdontologoDto> ObtenerPorIdAsync(int id);
        Task<List<HorarioOdontologoDto>> ObtenerPorOdontologoAsync(int odontologoId);

        // Método CRUD completo
        Task<int> GuardarAsync(HorarioOdontologoDtoIn dto); // Maneja Create y Update internamente
        Task EliminarAsync(int id);

        // Validación de negocio
        Task<bool> ValidarSolapamientoAsync(HorarioOdontologoDtoIn dto);
    }
}
