using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IRecordatorioBL
    {
        Task EnviarRecordatorioManualAsync(int citaId);
        Task<List<RecordatorioDto>> ObtenerBuzonPacienteAsync(int pacienteId);
    }
}
