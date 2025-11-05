using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObtenerPorEmailAsync(string email);
        Task<Usuario> AgregarAsync(Usuario usuario);
        Task<Usuario> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarPasswordAsync(int id, string nuevoPasswordHash);
        Task<bool> ActualizarAsync(Usuario usuario);
    }
}
