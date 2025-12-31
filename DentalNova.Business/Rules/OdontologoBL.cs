using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class OdontologoBL : IOdontologoBL
    {
        private readonly IRepository _repositorio;

        public OdontologoBL(IRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<PaginatedList<OdontologoDto>> ObtenerListaPaginadaAsync(OdontologoFilterDto filtro)
        {
            var query = _repositorio.Odontologo.ObtenerQueryableParaFiltro();

            // --- Filtros ---
            if (filtro.Id.HasValue)
                query = query.Where(o => o.Id == filtro.Id.Value);

            // Filtros de Usuario
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike))
                query = query.Where(o => o.Usuario.Nombre.Contains(filtro.NombreLike));
            if (!string.IsNullOrWhiteSpace(filtro.ApellidosLike))
                query = query.Where(o => o.Usuario.Apellidos.Contains(filtro.ApellidosLike));
            if (!string.IsNullOrWhiteSpace(filtro.CorreoLike))
                query = query.Where(o => o.Usuario.CorreoElectronico.Contains(filtro.CorreoLike));

            // Filtros de Odontólogo
            if (!string.IsNullOrWhiteSpace(filtro.CedulaLike))
                query = query.Where(o => o.CedulaProfesional.Contains(filtro.CedulaLike));
            if (filtro.FechaIngresoDesde.HasValue)
                query = query.Where(o => o.FechaIngreso.Date >= filtro.FechaIngresoDesde.Value);
            if (filtro.FechaIngresoHasta.HasValue)
                query = query.Where(o => o.FechaIngreso.Date <= filtro.FechaIngresoHasta.Value);

            // Filtro Especialidad (Muchos a Muchos)
            if (filtro.EspecialidadId.HasValue)
            {
                query = query.Where(o => o.Especialidades.Any(e => e.Id == filtro.EspecialidadId.Value));
            }

            // Ordenamiento
            query = query.OrderBy(o => o.Usuario.Apellidos);

            // Paginación y Mapeo
            var pagedEntities = await PaginatedList<Odontologo>.CreateAsync(query, filtro.Page, filtro.PageSize);
            var dtos = pagedEntities.Select(o => o.ToDto()).ToList();

            return PaginatedList<OdontologoDto>.Create(dtos, pagedEntities.TotalCount, pagedEntities.PageIndex, filtro.PageSize);
        }

        public async Task<OdontologoDto> ObtenerPorIdAdminAsync(int id)
        {
            var odontologo = await _repositorio.Odontologo.ObtenerDetalleCompletoAsync(id);
            return odontologo.ToDto(); // El mapeador ya incluye las listas de especialidades
        }

        public async Task CrearOdontologoAdminAsync(OdontologoDtoIn dto)
        {
            // Validar duplicados
            if (await _repositorio.Odontologo.ExisteOdontologoParaUsuarioAsync(dto.UsuarioId))
            {
                throw new InvalidOperationException("Este usuario ya está registrado como odontólogo.");
            }

            var nuevoOdontologo = new Odontologo
            {
                UsuarioId = dto.UsuarioId
            };
            nuevoOdontologo.MapFromDto(dto); // Mapea campos básicos

            // Asignar Especialidades
            if (dto.EspecialidadesIds != null && dto.EspecialidadesIds.Any())
            {
                var especialidades = await _repositorio.Especialidad.ObtenerPorIdsAsync(dto.EspecialidadesIds);
                nuevoOdontologo.Especialidades = especialidades;
            }

            await _repositorio.Odontologo.AgregarAsync(nuevoOdontologo);
        }

        public async Task ActualizarOdontologoAdminAsync(int id, OdontologoDtoIn dto)
        {
            var odontologo = await _repositorio.Odontologo.ObtenerDetalleCompletoAsync(id);
            if (odontologo == null) return;

            odontologo.MapFromDto(dto); // Actualiza campos básicos

            // Actualizar Especialidades (Limpiar y Reasignar)
            odontologo.Especialidades.Clear(); // Borra las relaciones actuales

            if (dto.EspecialidadesIds != null && dto.EspecialidadesIds.Any())
            {
                var nuevasEspecialidades = await _repositorio.Especialidad.ObtenerPorIdsAsync(dto.EspecialidadesIds);
                odontologo.Especialidades.AddRange(nuevasEspecialidades);
            }

            await _repositorio.Odontologo.ActualizarAsync(odontologo);
        }

        public async Task EliminarOdontologoAsync(int id)
        {
            await _repositorio.Odontologo.EliminarAsync(id);
        }

        // --- AUXILIARES UI ---

        public async Task<List<UsuarioDisponibleDto>> ObtenerUsuariosDisponiblesAsync(int? odontologoIdEdicion = null)
        {
            // IDs ocupados por odontólogos
            var idsOcupados = await _repositorio.Odontologo.ObtenerIdsUsuariosOcupadosAsync();
            // IDs ocupados por pacientes (tampoco queremos que un paciente sea odontólogo a la vez, ¿o sí?)
            var idsPacientes = await _repositorio.Paciente.ObtenerIdsUsuariosOcupadosAsync();

            var todosOcupados = idsOcupados.Union(idsPacientes).ToList();

            var queryUsuarios = _repositorio.Usuario.ObtenerQueryableParaFiltro()
                                                  .Where(u => u.Activo && u.Roles.Any(r => r.Nombre == "Odontologo"));

            if (odontologoIdEdicion.HasValue)
            {
                var actual = await _repositorio.Odontologo.ObtenerPorIdAsync(odontologoIdEdicion.Value);
                if (actual != null)
                {
                    queryUsuarios = queryUsuarios.Where(u => !todosOcupados.Contains(u.Id) || u.Id == actual.UsuarioId);
                }
            }
            else
            {
                queryUsuarios = queryUsuarios.Where(u => !todosOcupados.Contains(u.Id));
            }

            var usuarios = await queryUsuarios.OrderBy(u => u.Apellidos).ToListAsync();
            return usuarios.Select(u => u.ToDisponibleDto()).ToList();
        }

        public async Task<List<EspecialidadDto>> ObtenerTodasEspecialidadesAsync()
        {
            var especialidades = await _repositorio.Especialidad.ObtenerTodasAsync();
            return especialidades.Select(e => e.ToDto()).ToList();
        }
    }
}
