using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IPacienteService
    {
        // CRUD Básico
        Task<PagedResultDto<PacienteAdminDto>> ObtenerPacientesAsync(PacienteFilterDto filtro);
        Task<PacienteAdminDto> ObtenerPacientePorIdAsync(int id);
        Task CrearPacienteAsync(PacienteAdminDtoIn dto);
        Task ActualizarPacienteAsync(int id, PacienteAdminDtoIn dto);
        Task EliminarPacienteAsync(int id);

        // Auxiliar para el DropDownList
        Task<List<UsuarioDisponibleDto>> ObtenerUsuariosDisponiblesAsync(int? pacienteIdEdicion = null);
    }
}
