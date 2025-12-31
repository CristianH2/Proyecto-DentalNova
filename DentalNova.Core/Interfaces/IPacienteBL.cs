using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IPacienteBL
    {
        // --- Métodos (Perfil del Paciente) ---
        Task<PacienteDto> GuardarPerfilPacienteAsync(int usuarioId, PerfilPacienteDtoIn dto); // Busca por UsuarioId. Si no existe, crea. Si existe, actualiza.
        Task<PacienteDto> ObtenerPerfilPacienteAsync(int usuarioId);


        // --- Métodos (Gestión Admin) ---
        // Listado paginado con filtros
        Task<PaginatedList<PacienteAdminDto>> ObtenerListaPaginadaAsync(PacienteFilterDto filtro);

        // Obtener por ID (Detalles/Editar)
        Task<PacienteAdminDtoIn> ObtenerPorIdAdminAsync(int id);
        Task<PacienteAdminDto> ObtenerDetallePorIdAsync(int id); // Para la vista Details

        // CRUD
        Task CrearPacienteAdminAsync(PacienteAdminDtoIn dto);
        Task ActualizarPacienteAdminAsync(int id, PacienteAdminDtoIn dto);
        Task EliminarPacienteAsync(int id);

        // Lógica para el DropDownList (Usuarios no asignados)
        Task<List<UsuarioDisponibleDto>> ObtenerUsuariosDisponiblesAsync(int? pacienteIdEdicion = null);

    }
}
