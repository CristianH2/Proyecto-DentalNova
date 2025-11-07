using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface ICitaBL
    {
        Task<CitaAgendadaDto> AgendarCitaPacienteAsync(int usuarioId, CitaDtoIn dto);
        Task<IEnumerable<HistorialCitaDto>> ObtenerHistorialPacienteAsync(int usuarioId);
        Task<bool> CancelarCitaAsync(int usuarioId, int citaId);
    }
}
