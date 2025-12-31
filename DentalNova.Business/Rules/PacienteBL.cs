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
    public class PacienteBL : IPacienteBL
    {
        private readonly IRepository _repositorio;

        public PacienteBL(IRepository repositorio)
        {
            _repositorio = repositorio;
        }


        // Crea o actualiza el perfil de un Paciente asociado a un Usuario.
        // Calcula la edad automáticamente.
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

        // Obtiene el perfil de Paciente asociado a un Usuario.
        public async Task<PacienteDto> ObtenerPerfilPacienteAsync(int usuarioId)
        {
            var paciente = await _repositorio.Paciente.ObtenerPorUsuarioIdAsync(usuarioId);

            if (paciente == null)
            {
                return null; // El usuario no tiene un perfil de paciente
            }

            return paciente.ToDto();
        }

        // Calcula la edad basada en la fecha de nacimiento.
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


        // --- MÉTODOS PARA GESTIÓN ADMIN ---

        public async Task<PaginatedList<PacienteAdminDto>> ObtenerListaPaginadaAsync(PacienteFilterDto filtro)
        {
            // Obtener consulta base (Incluye Usuario)
            var query = _repositorio.Paciente.ObtenerQueryableParaFiltro();

            // Aplicar Filtros
            if (filtro.Id.HasValue)
                query = query.Where(p => p.Id == filtro.Id.Value);

            // Filtros de Usuario (Navegación)
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike))
                query = query.Where(p => p.Usuario.Nombre.Contains(filtro.NombreLike));
            if (!string.IsNullOrWhiteSpace(filtro.ApellidosLike))
                query = query.Where(p => p.Usuario.Apellidos.Contains(filtro.ApellidosLike));
            if (!string.IsNullOrWhiteSpace(filtro.CorreoLike))
                query = query.Where(p => p.Usuario.CorreoElectronico.Contains(filtro.CorreoLike));
            if (!string.IsNullOrWhiteSpace(filtro.TelefonoLike))
                query = query.Where(p => p.Usuario.Telefono.Contains(filtro.TelefonoLike));

            // Filtros de Paciente
            if (filtro.EdadMin.HasValue) query = query.Where(p => p.Edad >= filtro.EdadMin.Value);
            if (filtro.EdadMax.HasValue) query = query.Where(p => p.Edad <= filtro.EdadMax.Value);
            if (filtro.FechaDesde.HasValue) query = query.Where(p => p.FechaCreacion.Date >= filtro.FechaDesde.Value);
            if (filtro.FechaHasta.HasValue) query = query.Where(p => p.FechaCreacion.Date <= filtro.FechaHasta.Value);

            // Booleanos
            if (filtro.ConAlergias) query = query.Where(p => p.ConAlergias);
            if (filtro.ConEnfermedadesCronicas) query = query.Where(p => p.ConEnfermedadesCronicas);
            if (filtro.ConMedicamentosActuales) query = query.Where(p => p.ConMedicamentosActuales);
            if (filtro.ConAntecedentesFamiliares) query = query.Where(p => p.ConAntecedentesFamiliares);

            // Ordenamiento por defecto
            //query = query.OrderBy(p => p.Usuario.Apellidos);
            query = query.OrderBy(p => p.Id);

            // Paginación (Obtiene Entidades)
            var pagedEntities = await PaginatedList<Paciente>.CreateAsync(query, filtro.Page, filtro.PageSize);

            // Mapeo a DTOs
            var dtos = pagedEntities.Select(p => p.ToAdminDto()).ToList();

            // Reconstruir lista paginada con DTOs
            return PaginatedList<PacienteAdminDto>.Create(dtos, pagedEntities.TotalCount, pagedEntities.PageIndex, filtro.PageSize);
        }

        public async Task<List<UsuarioDisponibleDto>> ObtenerUsuariosDisponiblesAsync(int? pacienteIdEdicion = null)
        {
            // Obtener IDs ocupados
            var idsOcupados = await _repositorio.Paciente.ObtenerIdsUsuariosOcupadosAsync();

            // Obtener query de usuarios activos y que rol sea "Paciente"
            var queryUsuarios = _repositorio.Usuario.ObtenerQueryableParaFiltro()
                                                  .Where(u => u.Activo && u.Roles.Any(r => r.Nombre == "Paciente"));

            // 3. Aplicar filtro de exclusión
            if (pacienteIdEdicion.HasValue)
            {
                // Si estamos editando, necesitamos saber qué Usuario ID tiene este paciente para NO excluirlo
                var pacienteActual = await _repositorio.Paciente.ObtenerPorIdConUsuarioAsync(pacienteIdEdicion.Value);
                if (pacienteActual != null)
                {
                    // Excluir todos los ocupados EXCEPTO el del paciente actual
                    queryUsuarios = queryUsuarios.Where(u => !idsOcupados.Contains(u.Id) || u.Id == pacienteActual.UsuarioId);
                }
            }
            else
            {
                // Crear: Excluir todos los ocupados
                queryUsuarios = queryUsuarios.Where(u => !idsOcupados.Contains(u.Id));
            }

            // Ejecutar y Mapear
            var usuarios = await queryUsuarios.OrderBy(u => u.Apellidos).ToListAsync();
            return usuarios.Select(u => u.ToDisponibleDto()).ToList();
        }

        public async Task<PacienteAdminDtoIn> ObtenerPorIdAdminAsync(int id)
        {
            var paciente = await _repositorio.Paciente.ObtenerPorIdConUsuarioAsync(id);
            if (paciente == null) return null;

            // Mapeo manual inverso para el DTO de entrada (Edit)
            return new PacienteAdminDtoIn
            {
                Id = paciente.Id,
                UsuarioId = paciente.UsuarioId,
                ConAlergias = paciente.ConAlergias,
                Alergias = paciente.Alergias,
                // ... resto de campos
                Observaciones = paciente.Observaciones
            };
        }

        public async Task<PacienteAdminDto> ObtenerDetallePorIdAsync(int id)
        {
            var paciente = await _repositorio.Paciente.ObtenerPorIdConUsuarioAsync(id);
            return paciente.ToAdminDto();
        }

        public async Task CrearPacienteAdminAsync(PacienteAdminDtoIn dto)
        {
            // Validación de negocio: Race condition check
            if (await _repositorio.Paciente.ExistePacienteParaUsuarioAsync(dto.UsuarioId))
            {
                throw new InvalidOperationException("Este usuario ya tiene un expediente de paciente asignado.");
            }

            // Obtener usuario para calcular edad
            var usuario = await _repositorio.Usuario.ObtenerPorIdAsync(dto.UsuarioId);
            if (usuario == null || !usuario.FechaNacimiento.HasValue)
            {
                throw new InvalidOperationException("El usuario seleccionado no tiene fecha de nacimiento.");
            }

            var nuevoPaciente = new Paciente
            {
                UsuarioId = dto.UsuarioId,
                FechaCreacion = DateTime.UtcNow,
                Edad = CalcularEdad(usuario.FechaNacimiento.Value) // Reusamos el método privado existente
            };

            nuevoPaciente.MapFromAdminDto(dto); // Llenamos datos médicos

            await _repositorio.Paciente.AgregarAsync(nuevoPaciente);
        }

        public async Task ActualizarPacienteAdminAsync(int id, PacienteAdminDtoIn dto)
        {
            var paciente = await _repositorio.Paciente.ObtenerPorIdConUsuarioAsync(id);
            if (paciente == null) return;

            // Recalcular edad por si cambió la fecha de nacimiento del usuario o pasó el tiempo
            if (paciente.Usuario != null && paciente.Usuario.FechaNacimiento.HasValue)
            {
                paciente.Edad = CalcularEdad(paciente.Usuario.FechaNacimiento.Value);
            }

            paciente.FechaActualizacion = DateTime.UtcNow;
            paciente.MapFromAdminDto(dto); // Actualizamos datos médicos

            await _repositorio.Paciente.ActualizarAsync(paciente);
        }

        public async Task EliminarPacienteAsync(int id)
        {
            await _repositorio.Paciente.EliminarAsync(id);
        }

    }
}
