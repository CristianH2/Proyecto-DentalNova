using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IUsuarioBL
    {
        Task<TokenDto> LoginAsync(InicioDeSesionDto inicioDeSesion);
        Task<UsuarioDto> RegistrarAsync(UsuarioDtoIn registroDto);
        Task<bool> CambiarPasswordAsync(int usuarioId, CambioPasswordDtoIn cambioDto);
        Task<UsuarioDto> ActualizarPerfilUsuarioAsync(int usuarioId, PerfilUsuarioDtoIn dto);
    }
}
