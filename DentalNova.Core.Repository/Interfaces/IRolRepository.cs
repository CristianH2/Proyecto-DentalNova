using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IRolRepository
    {
        Task AgregarAsync(Rol rol);
        Task EliminarPorUsuarioIdAsync(int usuarioId);
    }
}
