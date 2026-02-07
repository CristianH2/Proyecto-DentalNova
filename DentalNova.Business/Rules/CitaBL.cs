using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.Daos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Business.Rules
{
    public class CitaBL : ICitaBL
    {
        private readonly IRepository _repositorio;

        public CitaBL(IRepository repositorio)
        {
            _repositorio = repositorio;
        }

        // --- LECTURA ---

        public async Task<CitaDto> ObtenerPorIdAsync(int id)
        {
            var entidad = await _repositorio.Cita.ObtenerPorIdAsync(id);
            return entidad.ToDto();
        }

        public async Task<PagedResultDto<CitaDto>> ObtenerListaPaginadaAsync(CitaFilterDto filtro, int page, int pageSize)
        {
            // Obtener Queryable base
            var query = _repositorio.Cita.ObtenerQueryable();

            // Aplicar Filtros
            if (filtro.PacienteId.HasValue)
                query = query.Where(x => x.PacienteId == filtro.PacienteId);
            if (filtro.OdontologoId.HasValue)
                query = query.Where(x => x.OdontologoId == filtro.OdontologoId);
            if (filtro.FechaInicio.HasValue)
                query = query.Where(x => x.FechaHora >= filtro.FechaInicio.Value);
            if (filtro.FechaFin.HasValue)
                query = query.Where(x => x.FechaHora <= filtro.FechaFin.Value);
            if (filtro.Estatus.HasValue)
                query = query.Where(x => x.EstatusCita == filtro.Estatus.Value);

            // Paginación
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var items = query
                .OrderByDescending(x => x.FechaHora)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CitaDto
                {
                    Id = x.Id,
                    PacienteId = x.PacienteId,
                    PacienteNombre = x.Paciente.Usuario.Nombre + " " + x.Paciente.Usuario.Apellidos,
                    OdontologoId = x.OdontologoId,
                    OdontologoNombre = x.Odontologo.Usuario.Nombre + " " + x.Odontologo.Usuario.Apellidos,
                    FechaHora = x.FechaHora,
                    DuracionMinutos = x.DuracionMinutos,
                    EstatusCita = x.EstatusCita,
                    MotivoConsulta = x.MotivoConsulta,
                    CostoTotal = x.CitasTratamientos.Sum(ct => ct.CostoFinal),

                    Tratamientos = x.CitasTratamientos.Select(t => new CitaTratamientoDto
                    {
                        TratamientoId = t.TratamientoId,
                        TratamientoNombre = t.Tratamiento.Nombre,
                        CostoFinal = t.CostoFinal,
                        Observaciones = t.Observaciones
                    }).ToList()
                })
                .ToList();

            return new PagedResultDto<CitaDto>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageIndex = page,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            };
        }

        // --- ESCRITURA ---

        public async Task<int> CrearAsync(CitaDtoIn dto)
        {
            await ValidarCita(dto);
            var cita = new Cita();

            cita.MapFromDto(dto);

            cita.EstatusCita = EstatusCita.Programada;
            cita.FechaCreacion = DateTime.Now;

            // Guardamos la cabecera para obtener el ID
            await _repositorio.Cita.AgregarAsync(cita);

            // Crear Detalles (Tratamientos)
            if (dto.TratamientosIds != null && dto.TratamientosIds.Any())
            {
                // USO DE DISTINCT: Evita que se duplique el registro
                var tratamientosUnicos = dto.TratamientosIds.Distinct().ToList();

                foreach (var tratamientoId in tratamientosUnicos)
                {
                    var tratamiento = await _repositorio.Tratamiento.ObtenerPorIdAsync(tratamientoId);
                    if (tratamiento != null)
                    {
                        // Calcular si es Inicial o Continuación
                        string notaAutomatica = await GenerarNotaTratamiento(dto.PacienteId, tratamientoId);

                        var detalle = new CitaTratamiento
                        {
                            CitaId = cita.Id,
                            TratamientoId = tratamiento.Id,
                            CostoFinal = tratamiento.Costo,

                            EstatusTratamiento = EstatusTratamiento.Pendiente,
                            Observaciones = notaAutomatica
                        };

                        await _repositorio.CitaTratamiento.AgregarAsync(detalle);
                    }
                }
            }

            return cita.Id;
        }

        public async Task ActualizarAsync(CitaDtoIn dto)
        {
            // 1. Obtenemos la cita CON sus detalles actuales
            var citaDb = await _repositorio.Cita.ObtenerPorIdAsync(dto.Id);
            if (citaDb == null) throw new Exception("La cita no existe.");

            // Validaciones de Cabecera (Fecha/Hora/Doctor)
            bool cambioHorario = citaDb.FechaHora != dto.FechaHora ||
                                 citaDb.OdontologoId != dto.OdontologoId ||
                                 citaDb.DuracionMinutos != dto.DuracionMinutos;

            if (cambioHorario) await ValidarCita(dto);

            citaDb.MapFromDto(dto);
            citaDb.FechaActualizacion = DateTime.Now;

            // Sincronización de tratamientos

            // Lista Limpia del Formulario (evitar duplicados de entrada)
            var idsNuevos = dto.TratamientosIds?.Distinct().ToList() ?? new List<int>();

            // Lista Actual en Base de Datos
            var tratamientosDb = citaDb.CitasTratamientos.ToList();
            var idsActuales = tratamientosDb.Select(x => x.TratamientoId).ToList();

            // ELIMINAR
            var detallesParaBorrar = tratamientosDb
                .Where(x => !idsNuevos.Contains(x.TratamientoId))
                .ToList();

            foreach (var item in detallesParaBorrar)
            {
                await _repositorio.CitaTratamiento.EliminarAsync(item.Id);
            }

            // AGREGAR
            var idsParaAgregar = idsNuevos
                .Where(id => !idsActuales.Contains(id))
                .ToList();

            foreach (var tratId in idsParaAgregar)
            {
                var tratamientoInfo = await _repositorio.Tratamiento.ObtenerPorIdAsync(tratId);
                if (tratamientoInfo != null)
                {
                    string nota = await GenerarNotaTratamiento(dto.PacienteId, tratId);

                    var nuevoDetalle = new CitaTratamiento
                    {
                        CitaId = citaDb.Id,
                        TratamientoId = tratamientoInfo.Id,
                        CostoFinal = tratamientoInfo.Costo, // Precio congelado al momento de agregar
                        EstatusTratamiento = EstatusTratamiento.Pendiente,
                        Observaciones = nota
                    };

                    await _repositorio.CitaTratamiento.AgregarAsync(nuevoDetalle);
                }
            }

            await _repositorio.Cita.ActualizarAsync(citaDb);
        }

        public async Task CambiarEstatusAsync(int id, EstatusCita nuevoEstatus)
        {
            var cita = await _repositorio.Cita.ObtenerPorIdAsync(id);
            if (cita == null) throw new Exception("La cita no existe.");

            // Regla de Negocio: REACTIVACIÓN DE CITA
            bool estabaInactiva = cita.EstatusCita == EstatusCita.Cancelada ||
                                  cita.EstatusCita == EstatusCita.NoAsistida;

            bool seraActiva = nuevoEstatus == EstatusCita.Programada;

            if (estabaInactiva && seraActiva)
            {
                // Calculamos la hora fin original
                var fechaFin = cita.FechaHora.AddMinutes((int)cita.DuracionMinutos);

                // Verificamos si alguien más tomó ese lugar mientras estuvo cancelada
                var hayConflicto = await _repositorio.Cita.ExisteConflictoHorarioAsync(
                    cita.OdontologoId,
                    cita.FechaHora,
                    fechaFin,
                    cita.Id
                );

                if (hayConflicto)
                {
                    throw new Exception("No se puede reactivar la cita: El horario original ya ha sido ocupado por otra cita.");
                }
            }

            // Actualizar estado y fecha de modificación
            cita.EstatusCita = nuevoEstatus;
            cita.FechaActualizacion = DateTime.Now;
             
            await _repositorio.Cita.ActualizarAsync(cita);
        }

        public async Task EliminarAsync(int id)
        {
            var cita = await _repositorio.Cita.ObtenerPorIdAsync(id);
            if (cita == null) throw new Exception("La cita no existe.");

            if (cita.EstatusCita == EstatusCita.Completada)
            {
                throw new Exception("No se puede eliminar una cita que ya fue completada.");
            }

            // Soft Delete: cambia estatus a Cancelada
            await _repositorio.Cita.EliminarAsync(id);
        }

        // --- HELPERS PRIVADOS ---

        private async Task ValidarCita(CitaDtoIn dto)

        {
            // Validar Fechas coherentes
            if (dto.FechaHora < DateTime.Now.AddMinutes(-10))
                throw new Exception("No se pueden agendar citas en el pasado.");

            var fechaFin = dto.FechaHora.AddMinutes((int)dto.DuracionMinutos);

            // Validar Horario Laboral ¿El doctor trabaja a esa hora?
            var esLaboral = await _repositorio.Cita.EsHorarioLaboralValidoAsync(dto.OdontologoId, dto.FechaHora, fechaFin);

            if (!esLaboral)
                throw new Exception("El horario seleccionado está fuera del turno laboral del odontólogo.");

            // Validar Conflictos ¿Ya tiene cita a esa hora?
            var hayConflicto = await _repositorio.Cita.ExisteConflictoHorarioAsync(dto.OdontologoId, dto.FechaHora, fechaFin, dto.Id == 0 ? null : dto.Id);

            if (hayConflicto)
                throw new Exception("El odontólogo ya tiene una cita agendada o solapada en ese horario.");

        }

        private async Task<string> GenerarNotaTratamiento(int pacienteId, int tratamientoId)
        {
            // Verificamos historial
            int conteoPrevio = await _repositorio.CitaTratamiento
                .ContarTratamientosPreviosAsync(pacienteId, tratamientoId);

            if (conteoPrevio == 0)
            {
                return "Tratamiento inicial de diagnóstico.";
            }
            else
            {
                return $"Continuación de tratamiento (Sesión #{conteoPrevio + 1}).";
            }
        }

    }
}
