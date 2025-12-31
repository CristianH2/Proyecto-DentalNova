using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IAuthService
    {
        // Devuelve el Token (string) si es exitoso, o lanza excepción si falla
        Task<TokenDto> LoginAsync(InicioDeSesionDto dto);
    }
}
