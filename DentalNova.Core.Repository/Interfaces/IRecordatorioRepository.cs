using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IRecordatorioRepository
    {
        Task AgregarAsync(Recordatorio recordatorio);
        Task<List<Recordatorio>> ObtenerPorPacienteAsync(int pacienteId);
        Task<bool> ExisteRecordatorioParaCitaAsync(int citaId);
    }
}
