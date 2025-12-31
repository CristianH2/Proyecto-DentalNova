using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IOdontologoRepository
    {
        Task<Odontologo> ObtenerPorIdAsync(int id); // Para obtener los datos del odontólogo asignado

        IQueryable<Odontologo> ObtenerQueryableParaFiltro();

        // Obtiene Odontólogo con Usuario Y Especialidades
        Task<Odontologo> ObtenerDetalleCompletoAsync(int id);

        Task AgregarAsync(Odontologo odontologo);
        Task ActualizarAsync(Odontologo odontologo);
        Task EliminarAsync(int id);

        // Validaciones
        Task<bool> ExisteOdontologoParaUsuarioAsync(int usuarioId);
        Task<List<int>> ObtenerIdsUsuariosOcupadosAsync();
    }
}
