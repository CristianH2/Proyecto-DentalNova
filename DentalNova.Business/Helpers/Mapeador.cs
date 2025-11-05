using DentalNova.Core.Dtos;
using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Helpers
{
    public static class Mapeador
    {
        /// <summary>
        /// Convierte UsuarioDtoIn a Usuario
        /// </summary>
        public static Usuario ToEntidad(this UsuarioDtoIn dto)
        {
            if (dto == null) return null;

            return new Usuario
            {
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                CorreoElectronico = dto.CorreoElectronico,
                CURP = dto.CURP,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Telefono = dto.Telefono,
                FechaNacimiento = dto.FechaNacimiento,
                Genero = dto.Genero,
                Activo = true,
                // Las colecciones (Roles, LogActividades) se inicializan vacías
                Roles = new List<Rol>(),
                LogActividades = new List<LogActividad>()
            };
        }

        /// <summary>
        /// Convierte la Entidad Usuario al DTO (salida)
        /// </summary>
        public static UsuarioDto ToDto(this Usuario entidad)
        {
            if (entidad == null) return null;

            return new UsuarioDto
            {
                Id = entidad.Id,
                NombreCompleto = $"{entidad.Nombre} {entidad.Apellidos}",
                CorreoElectronico = entidad.CorreoElectronico,
                CURP = entidad.CURP,
                // Mapea la lista de entidades Rol a una simple lista de strings (nombres de rol)
                Roles = entidad.Roles?.Select(r => r.Nombre).ToList() ?? new List<string>()
            };
        }

        /// <summary>
        /// Convierte la Entidad Paciente al DTO (salida)
        /// </summary>
        public static PacienteDto ToDto(this Paciente entidad)
        {
            if (entidad == null) return null;

            return new PacienteDto
            {
                Id = entidad.Id,
                Edad = entidad.Edad,
                ConAlergias = entidad.ConAlergias,
                Alergias = entidad.Alergias,
                ConEnfermedadesCronicas = entidad.ConEnfermedadesCronicas,
                EnfermedadesCronicas = entidad.EnfermedadesCronicas,
                ConMedicamentosActuales = entidad.ConMedicamentosActuales,
                MedicamentosActuales = entidad.MedicamentosActuales,
                ConAntecedentesFamiliares = entidad.ConAntecedentesFamiliares,
                AntecedentesFamiliares = entidad.AntecedentesFamiliares,
                Observaciones = entidad.Observaciones,
                FechaCreacion = entidad.FechaCreacion,
                FechaActualizacion = entidad.FechaActualizacion
            };
        }
    }
}
