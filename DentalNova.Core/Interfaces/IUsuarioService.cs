using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    // Define qué operaciones puede pedir el MVC a la API
    public interface IUsuarioService
    {
        Task<PagedResultDto<UsuarioAdminDto>> ObtenerUsuariosAsync(UsuarioFilterDto filtro);
        Task<UsuarioAdminDto> ObtenerUsuarioPorIdAsync(int id);
        Task CrearUsuarioAsync(UsuarioAdminDtoIn dto);
        Task ActualizarUsuarioAsync(int id, UsuarioAdminDtoIn dto);
        Task EliminarUsuarioAsync(int id);
        Task<string> ObtenerFechaNacimientoAsync(int id);
    }
}
