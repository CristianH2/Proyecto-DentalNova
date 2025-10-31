using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Security
{
    public interface ITokenService
    {
        string GenerarToken(Usuario usuario);
    }
}
