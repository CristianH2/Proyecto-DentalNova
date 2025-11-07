using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface ICitaRepository
    {
        Task<Cita> AgregarAsync(Cita cita);
        Task<List<Cita>> ObtenerCitasEnRangoAsync(List<int> odontologoIds, DateTime inicio, DateTime fin); // Para buscar colisiones de horario
        Task<List<Cita>> ObtenerHistorialPorPacienteIdAsync(int pacienteId);
        Task<Cita> ObtenerPorIdAsync(int citaId);
        Task<bool> ActualizarAsync(Cita cita);
    }
}
