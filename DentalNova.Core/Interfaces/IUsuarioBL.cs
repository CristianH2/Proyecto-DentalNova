using DentalNova.Core.Dtos;
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


    }
}
