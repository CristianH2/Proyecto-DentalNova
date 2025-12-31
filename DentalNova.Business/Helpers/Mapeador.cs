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
        // Convierte UsuarioDtoIn a Usuario
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

        // Convierte la Entidad Usuario al DTO (salida)
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

        // De Entidad -> DTO de Salida Admin
        public static UsuarioAdminDto ToAdminDto(this Usuario entidad)
        {
            if (entidad == null) return null;
            return new UsuarioAdminDto
            {
                Id = entidad.Id,
                Nombre = entidad.Nombre,
                Apellidos = entidad.Apellidos,
                CorreoElectronico = entidad.CorreoElectronico,
                CURP = entidad.CURP,
                Telefono = entidad.Telefono,
                FechaNacimiento = entidad.FechaNacimiento,
                Genero = entidad.Genero,
                Activo = entidad.Activo,
                // Aseguramos que Roles no sea null
                Roles = entidad.Roles?.Select(r => r.Nombre).ToList() ?? new List<string>()
            };
        }

        // De DTO de Entrada Admin -> Entidad (Para Crear/Editar)
        public static void MapFromAdminDto(this Usuario entidad, UsuarioAdminDtoIn dto)
        {
            entidad.Nombre = dto.Nombre;
            entidad.Apellidos = dto.Apellidos;
            entidad.CorreoElectronico = dto.CorreoElectronico;
            entidad.CURP = dto.CURP;
            entidad.Telefono = dto.Telefono;
            entidad.FechaNacimiento = dto.FechaNacimiento;
            entidad.Genero = dto.Genero;
            entidad.Activo = dto.Activo;
        }

        // Convierte la Entidad Paciente al DTO (salida)
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

        public static PacienteAdminDto ToAdminDto(this Paciente entidad)
        {
            if (entidad == null) return null;
            return new PacienteAdminDto
            {
                Id = entidad.Id,
                UsuarioId = entidad.UsuarioId,
                // Datos planos del usuario para la tabla
                Nombre = entidad.Usuario?.Nombre ?? "N/A",
                Apellidos = entidad.Usuario?.Apellidos ?? "N/A",
                CorreoElectronico = entidad.Usuario?.CorreoElectronico ?? "N/A",
                Telefono = entidad.Usuario?.Telefono,
                // Datos del paciente
                Edad = entidad.Edad,
                FechaCreacion = entidad.FechaCreacion,
                ConAlergias = entidad.ConAlergias,
                ConEnfermedadesCronicas = entidad.ConEnfermedadesCronicas ,
                ConMedicamentosActuales = entidad.ConMedicamentosActuales,
                ConAntecedentesFamiliares = entidad.ConAntecedentesFamiliares,
                Alergias = entidad.Alergias,
                EnfermedadesCronicas = entidad.EnfermedadesCronicas,
                MedicamentosActuales = entidad.MedicamentosActuales,
                AntecedentesFamiliares = entidad.AntecedentesFamiliares,
                Observaciones = entidad.Observaciones
            };
        }

        public static UsuarioDisponibleDto ToDisponibleDto(this Usuario entidad)
        {
            return new UsuarioDisponibleDto
            {
                Id = entidad.Id,
                NombreCompleto = $"{entidad.Apellidos}, {entidad.Nombre}",
                Correo = entidad.CorreoElectronico,
                FechaNacimiento = entidad.FechaNacimiento
            };
        }

        public static void MapFromAdminDto(this Paciente entidad, PacienteAdminDtoIn dto)
        {
            // No mapeamos ID ni UsuarioId aquí (se manejan aparte en BL)
            entidad.ConAlergias = dto.ConAlergias;
            entidad.Alergias = dto.Alergias;
            entidad.ConEnfermedadesCronicas = dto.ConEnfermedadesCronicas;
            entidad.EnfermedadesCronicas = dto.EnfermedadesCronicas;
            entidad.ConMedicamentosActuales = dto.ConMedicamentosActuales;
            entidad.MedicamentosActuales = dto.MedicamentosActuales;
            entidad.ConAntecedentesFamiliares = dto.ConAntecedentesFamiliares;
            entidad.AntecedentesFamiliares = dto.AntecedentesFamiliares;
            entidad.Observaciones = dto.Observaciones;
        }

        public static OdontologoDto ToDto(this Odontologo entidad)
        {
            if (entidad == null) return null;
            return new OdontologoDto
            {
                Id = entidad.Id,
                UsuarioId = entidad.UsuarioId,
                // Datos del Usuario
                Nombre = entidad.Usuario?.Nombre ?? "N/A",
                Apellidos = entidad.Usuario?.Apellidos ?? "N/A",
                CorreoElectronico = entidad.Usuario?.CorreoElectronico ?? "N/A",
                Telefono = entidad.Usuario?.Telefono,
                // Datos del Odontólogo
                CedulaProfesional = entidad.CedulaProfesional,
                AnioGraduacion = entidad.AnioGraduacion,
                Institucion = entidad.Institucion,
                FechaIngreso = entidad.FechaIngreso,
                // Mapeo de Especialidades (Nombres e IDs)
                Especialidades = entidad.Especialidades?.Select(e => e.Nombre).ToList() ?? new List<string>(),
                EspecialidadesIds = entidad.Especialidades?.Select(e => e.Id).ToList() ?? new List<int>()
            };
        }

        public static EspecialidadDto ToDto(this Especialidad entidad)
        {
            return new EspecialidadDto
            {
                Id = entidad.Id,
                Nombre = entidad.Nombre,
                //Descripcion = entidad.Descripcion
            };
        }

        public static void MapFromDto(this Odontologo entidad, OdontologoDtoIn dto)
        {
            entidad.CedulaProfesional = dto.CedulaProfesional;
            entidad.AnioGraduacion = dto.AnioGraduacion;
            entidad.Institucion = dto.Institucion;
            entidad.FechaIngreso = dto.FechaIngreso;
            // Nota: Las especialidades se manejan manualmente en el BL, no aquí.
        }

        public static TratamientoDto ToDto(this Tratamiento entidad)
        {
            return new TratamientoDto
            {
                Id = entidad.Id,
                Nombre = entidad.Nombre,
                Descripcion = entidad.Descripcion,
                Costo = entidad.Costo,
                DuracionDias = entidad.DuracionDias,
                Activo = entidad.Activo
            };
        }

        public static void MapFromDto(this Tratamiento entidad, TratamientoDtoIn dto)
        {
            entidad.Nombre = dto.Nombre;
            entidad.Descripcion = dto.Descripcion;
            entidad.Costo = dto.Costo;
            entidad.DuracionDias = dto.DuracionDias;
            entidad.Activo = dto.Activo;
        }
        
    }
}
