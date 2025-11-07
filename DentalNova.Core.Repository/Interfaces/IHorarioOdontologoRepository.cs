using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IHorarioOdontologoRepository
    {
        // Para encontrar odontólogos que *están trabajando* ese día y hora
        Task<List<HorarioOdontologo>> ObtenerHorariosDisponiblesAsync(Enumerables.DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin);
    }
}
