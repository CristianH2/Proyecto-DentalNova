using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DentalNova.Core.Interfaces
{
    public interface IAuthBL
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
    }
}
