using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class PacienteBL : IPacienteBL
    {
        private readonly IRepositoriy _repositorio;

        public PacienteBL(IRepositoriy repositorio)
        {
            _repositorio = repositorio;
        }

        /// <summary>
        /// Crea o actualiza el perfil de un Paciente asociado a un Usuario.
        /// Calcula la edad automáticamente.
        /// </summary>
        public async Task<PacienteDto> GuardarPerfilPacienteAsync(int usuarioId, PerfilPacienteDtoIn dto)
        {
            // Obtener el Usuario (para su Fecha de Nacimiento)
            var usuario = await _repositorio.Usuario.ObtenerPorIdAsync(usuarioId);
            if (usuario == null || usuario.FechaNacimiento == null)
            {
                throw new InvalidOperationException("El usuario no tiene una fecha de nacimiento registrada para calcular la edad.");
            }

            // Calcular la Edad
            int edadCalculada = CalcularEdad(usuario.FechaNacimiento.Value);

            // Buscar si el perfil de Paciente ya existe
            var pacienteExistente = await _repositorio.Paciente.ObtenerPorUsuarioIdAsync(usuarioId);

            Paciente pacienteGuardado;

            if (pacienteExistente == null)
            {
                // NO EXISTE (CREAR)
                var nuevoPaciente = new Paciente
                {
                    UsuarioId = usuarioId,
                    Edad = edadCalculada,
                    FechaCreacion = DateTime.Now,
                    // Mapeo de campos del DTO
                    ConAlergias = dto.ConAlergias,
                    Alergias = dto.Alergias,
                    ConEnfermedadesCronicas = dto.ConEnfermedadesCronicas,
                    EnfermedadesCronicas = dto.EnfermedadesCronicas,
                    ConMedicamentosActuales = dto.ConMedicamentosActuales,
                    MedicamentosActuales = dto.MedicamentosActuales,
                    ConAntecedentesFamiliares = dto.ConAntecedentesFamiliares,
                    AntecedentesFamiliares = dto.AntecedentesFamiliares,
                    Observaciones = dto.Observaciones
                };

                pacienteGuardado = await _repositorio.Paciente.AgregarAsync(nuevoPaciente);
            }
            else
            {
                // SÍ EXISTE (ACTUALIZAR)
                pacienteExistente.Edad = edadCalculada; // Recalcula la edad
                pacienteExistente.FechaActualizacion = DateTime.Now;
                // Mapeo de campos del DTO
                pacienteExistente.ConAlergias = dto.ConAlergias;
                pacienteExistente.Alergias = dto.Alergias;
                pacienteExistente.ConEnfermedadesCronicas = dto.ConEnfermedadesCronicas;
                pacienteExistente.EnfermedadesCronicas = dto.EnfermedadesCronicas;
                pacienteExistente.ConMedicamentosActuales = dto.ConMedicamentosActuales;
                pacienteExistente.MedicamentosActuales = dto.MedicamentosActuales;
                pacienteExistente.ConAntecedentesFamiliares = dto.ConAntecedentesFamiliares;
                pacienteExistente.AntecedentesFamiliares = dto.AntecedentesFamiliares;
                pacienteExistente.Observaciones = dto.Observaciones;

                pacienteGuardado = await _repositorio.Paciente.ActualizarAsync(pacienteExistente);
            }

            // 5. Devolver el DTO de salida
            return pacienteGuardado.ToDto();
        }

        /// <summary>
        /// Obtiene el perfil de Paciente asociado a un Usuario.
        /// </summary>
        public async Task<PacienteDto> ObtenerPerfilPacienteAsync(int usuarioId)
        {
            var paciente = await _repositorio.Paciente.ObtenerPorUsuarioIdAsync(usuarioId);

            if (paciente == null)
            {
                return null; // El usuario no tiene un perfil de paciente
            }

            return paciente.ToDto();
        }

        /// <summary>
        /// Calcula la edad basada en la fecha de nacimiento.
        /// </summary>
        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Now;
            int edad = hoy.Year - fechaNacimiento.Year;
            // Ajusta por si aún no cumple años este año
            if (fechaNacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }
            return edad;
        }
    }
}
