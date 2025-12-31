using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IUsuarioBL
    {
        // --- Métodos de Paciente ---
        Task<Usuario> ValidarCredencialesAsync(InicioDeSesionDto inicioDeSesion);
        Task<UsuarioDto> RegistrarAsync(UsuarioDtoIn registroDto);
        Task<bool> CambiarPasswordAsync(int usuarioId, CambioPasswordDtoIn cambioDto);
        Task<UsuarioDto> ActualizarPerfilUsuarioAsync(int usuarioId, PerfilUsuarioDtoIn dto);
        Task<UsuarioDto> ObtenerPerfilUsuarioAsync(int usuarioId);

        // --- Métodos nuevos para Admin MVC---
        Task<PaginatedList<Usuario>> ObtenerListaPaginadaAsync(UsuarioFilterDto filtro);
        Task<Usuario> ObtenerPorIdAdminAsync(int id);
        Task<string> ObtenerFechaNacimientoJsonAsync(int id);
        Task CrearUsuarioAdminAsync(Usuario usuario, string newPassword, List<string> rolesSeleccionados);
        Task ActualizarUsuarioAdminAsync(Usuario usuario, string? newPassword, List<string> rolesSeleccionados);
        Task EliminarUsuarioAsync(int id);

        // Métodos de validación (los moveremos aquí desde el controlador)
        Task<bool> EmailYaExisteAsync(string email, int? usuarioId = null);
        Task<bool> CurpYaExisteAsync(string curp, int? usuarioId = null);

    }
}
