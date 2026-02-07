using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IHorarioOdontologoRepository
    {

        // --- Consultas Específicas ---
        Task<IEnumerable<HorarioOdontologo>> ObtenerPorOdontologoIdAsync(int odontologoId);
        Task<HorarioOdontologo> ObtenerPorIdAsync(int id);
        Task<List<HorarioOdontologo>> ObtenerHorariosDisponiblesAsync(DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin);

        // --- CRUD ---
        IQueryable<HorarioOdontologo> ObtenerQueryableParaFiltro();
        Task AgregarAsync(HorarioOdontologo horario);
        Task ActualizarAsync(HorarioOdontologo horario);
        Task EliminarAsync(int id);

        // --- Validaciones ---
        Task<bool> ExisteSolapamientoAsync(int odontologoId, DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null);
    }
}
