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
    }
}
