using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IPacienteBL
    {
        Task<PacienteDto> GuardarPerfilPacienteAsync(int usuarioId, PerfilPacienteDtoIn dto); // Busca por UsuarioId. Si no existe, crea. Si existe, actualiza.
        Task<PacienteDto> ObtenerPerfilPacienteAsync(int usuarioId);
    }
}
