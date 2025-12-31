using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IPacienteRepository
    {
        Task<Paciente> ObtenerPorUsuarioIdAsync(int usuarioId);
        Task<Paciente> AgregarAsync(Paciente paciente);
        Task<Paciente> ActualizarAsync(Paciente paciente);

        // Para el filtro avanzado
        IQueryable<Paciente> ObtenerQueryableParaFiltro();

        // Para obtener por ID incluyendo datos de Usuario
        Task<Paciente> ObtenerPorIdConUsuarioAsync(int id);

        // Para eliminar
        Task EliminarAsync(int id);

        // Para verificar disponibilidad (evitar race conditions)
        Task<bool> ExistePacienteParaUsuarioAsync(int usuarioId);

        // Para obtener la lista de IDs de usuarios que YA son pacientes
        Task<List<int>> ObtenerIdsUsuariosOcupadosAsync();
    }
}
