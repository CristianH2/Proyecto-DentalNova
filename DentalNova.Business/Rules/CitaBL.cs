using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using static DentalNova.Core.Repository.Entities.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class CitaBL : ICitaBL
    {
        private readonly IRepository _repositorio;
        private const int DIAGNOSTICO_TRATAMIENTO_ID = 4; // ID del diagnóstico
        private const DuracionMinutos DURACION_DEFAULT = DuracionMinutos.Treinta; // Duración default

        public CitaBL(IRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<CitaAgendadaDto> AgendarCitaPacienteAsync(int usuarioId, CitaDtoIn dto)
        {
            // --- VALIDACIÓN INICIAL ---
            var paciente = await _repositorio.Paciente.ObtenerPorUsuarioIdAsync(usuarioId);
            if (paciente == null)
            {
                throw new InvalidOperationException("No se encontró un perfil de paciente para este usuario.");
            }

            var tratamientoDiagnostico = await _repositorio.Tratamiento.ObtenerPorIdAsync(DIAGNOSTICO_TRATAMIENTO_ID);
            if (tratamientoDiagnostico == null)
            {
                throw new InvalidOperationException("El tratamiento de diagnóstico (ID 4) no se encuentra en la base de datos.");
            }

            // --- LÓGICA DE DISPONIBILIDAD ---
            var inicioCita = dto.FechaHora;
            var duracionCita = TimeSpan.FromMinutes((int)DURACION_DEFAULT);
            var finCita = inicioCita.Add(duracionCita);
            var diaSemana = ConvertirDiaSemana(inicioCita.DayOfWeek);

            // Encontrar odontólogos que *están trabajando* a esa hora
            var horariosDisponibles = await _repositorio.HorarioOdontologo.ObtenerHorariosDisponiblesAsync(diaSemana, inicioCita.TimeOfDay, finCita.TimeOfDay);
            var odontologosConHorarioIds = horariosDisponibles.Select(h => h.Odontologo.Id).Distinct().ToList();

            if (!odontologosConHorarioIds.Any())
            {
                throw new InvalidOperationException("No hay odontólogos con horario disponible para la fecha y hora seleccionadas.");
            }

            // Verificar que esos odontólogos no tengan *otra cita* chocando
            var citasEnRango = await _repositorio.Cita.ObtenerCitasEnRangoAsync(odontologosConHorarioIds, inicioCita, finCita);
            var odontologosOcupadosIds = citasEnRango.Select(c => c.OdontologoId).Distinct();

            var odontologosLibresIds = odontologosConHorarioIds.Except(odontologosOcupadosIds);

            if (!odontologosLibresIds.Any())
            {
                throw new InvalidOperationException("Todos los odontólogos disponibles ya tienen citas asignadas en ese horario.");
            }

            // Asignar el primer odontólogo libre
            var odontologoAsignadoId = odontologosLibresIds.First();
            var odontologoAsignado = await _repositorio.Odontologo.ObtenerPorIdAsync(odontologoAsignadoId);

            // --- CREAR LAS ENTIDADES ---

            // Crear la Cita
            var nuevaCita = new Cita
            {
                FechaHora = inicioCita,
                DuracionMinutos = DURACION_DEFAULT,
                EstatusCita = EstatusCita.Programada, // Default
                MotivoConsulta = dto.MotivoConsulta,
                FechaCreacion = DateTime.Now,
                PacienteId = paciente.Id,
                OdontologoId = odontologoAsignadoId
            };

            // Guarda la cita (esto genera el nuevo CitaId)
            await _repositorio.Cita.AgregarAsync(nuevaCita);

            // Crear el CitaTratamiento
            var nuevoCitaTratamiento = new CitaTratamiento
            {
                CitaId = nuevaCita.Id,
                TratamientoId = DIAGNOSTICO_TRATAMIENTO_ID,
                CostoFinal = tratamientoDiagnostico.Costo,
                EstatusTratamiento = EstatusTratamiento.Pendiente, // Default
                Observaciones = "Tratamiento inicial de diagnóstico."
            };

            // Guarda el tratamiento de la cita
            await _repositorio.CitaTratamiento.AgregarAsync(nuevoCitaTratamiento);

            // --- 4. DEVOLVER RESPUESTA (DTO) ---
            return new CitaAgendadaDto
            {
                Id = nuevaCita.Id,
                FechaHora = nuevaCita.FechaHora,
                Estatus = nuevaCita.EstatusCita.ToString(),
                MotivoConsulta = nuevaCita.MotivoConsulta,
                // Usamos el nombre del Usuario asociado al Odontólogo
                OdontologoAsignado = $"{odontologoAsignado.Usuario.Nombre} {odontologoAsignado.Usuario.Apellidos}",
                TratamientoInicial = tratamientoDiagnostico.Nombre
            };
        }

        /// <summary>
        /// Convierte el DayOfWeek de .NET (Domingo=0) al enum de la BD (Lunes=1... Domingo=7)
        /// </summary>
        private DiaSemana ConvertirDiaSemana(DayOfWeek dia)
        {
            if (dia == DayOfWeek.Sunday)
                return DiaSemana.Domingo;

            return (DiaSemana)dia;
        }

        public async Task<IEnumerable<HistorialCitaDto>> ObtenerHistorialPacienteAsync(int usuarioId)
        {
            // Obtener el PacienteId a partir del UsuarioId
            var paciente = await _repositorio.Paciente.ObtenerPorUsuarioIdAsync(usuarioId);
            if (paciente == null)
            {
                return new List<HistorialCitaDto>(); // Si no hay perfil de paciente, no hay historial.
            }

            // Llamar al repositorio para obtener los datos
            var citasEntidades = await _repositorio.Cita.ObtenerHistorialPorPacienteIdAsync(paciente.Id);

            // Mapear las Entidades
            var historialDto = new List<HistorialCitaDto>();

            foreach (var cita in citasEntidades)
            {
                var citaDto = new HistorialCitaDto
                {
                    Id = cita.Id,
                    FechaHora = cita.FechaHora,
                    Estatus = cita.EstatusCita,
                    MotivoConsulta = cita.MotivoConsulta,

                    // Asigna el nombre del odontólogo
                    OdontologoAsignado = (cita.Odontologo?.Usuario != null)
                        ? $"{cita.Odontologo.Usuario.Nombre} {cita.Odontologo.Usuario.Apellidos}"
                        : "No Asignado",

                    // Usa la propiedad calculada de la entidad
                    CostoTotal = cita.CostoTotalTratamientos,

                    // Mapea la lista anidada de tratamientos
                    Tratamientos = cita.CitasTratamientos?.Select(ct => new HistorialTratamientoDto
                    {
                        NombreTratamiento = ct.Tratamiento?.Nombre ?? "Tratamiento no especificado",
                        Estatus = ct.EstatusTratamiento,
                        CostoFinal = ct.CostoFinal,
                        Observaciones = ct.Observaciones
                    }).ToList() ?? new List<HistorialTratamientoDto>()
                };

                historialDto.Add(citaDto);
            }

            return historialDto;
        }

        public async Task<bool> CancelarCitaAsync(int usuarioId, int citaId)
        {
            // Obtener el PacienteId
            var paciente = await _repositorio.Paciente.ObtenerPorUsuarioIdAsync(usuarioId);
            if (paciente == null)
            {
                throw new InvalidOperationException("Este usuario no tiene un perfil de paciente.");
            }

            // Obtener la cita que se desea cancelar
            var cita = await _repositorio.Cita.ObtenerPorIdAsync(citaId);
            if (cita == null)
            {
                throw new KeyNotFoundException("La cita solicitada no existe.");
            }

            // ¿Es el dueño de la cita?
            if (cita.PacienteId != paciente.Id)
            {
                throw new UnauthorizedAccessException("No tiene permiso para cancelar esta cita.");
            }

            // ¿Se puede cancelar?
            if (cita.EstatusCita == EstatusCita.Completada || cita.EstatusCita == EstatusCita.Cancelada)
            {
                throw new InvalidOperationException($"La cita ya está '{cita.EstatusCita}' y no puede ser cancelada.");
            }

            // Cambiar el estado y guardar
            cita.EstatusCita = EstatusCita.Cancelada;
            cita.FechaActualizacion = DateTime.Now;

            return await _repositorio.Cita.ActualizarAsync(cita);
        }
    }
}
