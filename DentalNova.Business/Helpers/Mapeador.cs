using DentalNova.Business.Rules;
using DentalNova.Core.Dtos;
using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;
using static System.Net.Mime.MediaTypeNames;

namespace DentalNova.Business.Helpers
{
    public static class Mapeador
    {
        // ---- Usuario Mappings --- //
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

        // ---- UsuarioAdmin Mappings --- //
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

        public static void MapFromDto(this Usuario entidad, UsuarioAdminDtoIn dto)
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

        // ---- Paciente Mappings --- //
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
                Genero = entidad.Usuario?.Genero ?? '-' ,
                Edad = entidad.Edad,
                FechaCreacion = entidad.FechaCreacion,
                ConAlergias = entidad.ConAlergias,
                ConEnfermedadesCronicas = entidad.ConEnfermedadesCronicas,
                ConMedicamentosActuales = entidad.ConMedicamentosActuales,
                ConAntecedentesFamiliares = entidad.ConAntecedentesFamiliares,
                Alergias = entidad.Alergias,
                EnfermedadesCronicas = entidad.EnfermedadesCronicas,
                MedicamentosActuales = entidad.MedicamentosActuales,
                AntecedentesFamiliares = entidad.AntecedentesFamiliares,
                Observaciones = entidad.Observaciones
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

        // ---- UsuarioDisponible Mappings --- //
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

        // ---- Odontologo Mappings --- //
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
                Activo = entidad.Usuario?.Activo ?? false,
                // Mapeo de Especialidades (Nombres e IDs)
                Especialidades = entidad.Especialidades?.Select(e => e.Nombre).ToList() ?? new List<string>(),
                EspecialidadesIds = entidad.Especialidades?.Select(e => e.Id).ToList() ?? new List<int>()
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

        // ---- Especialidad Mappings --- //
        public static EspecialidadDto ToDto(this Especialidad entidad)
        {
            return new EspecialidadDto
            {
                Id = entidad.Id,
                Nombre = entidad.Nombre,
                //Descripcion = entidad.Descripcion
            };
        }


        // ---- Tratamiento Mappings --- //
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


        // ---- HorarioOdontologo Mappings --- //
        public static HorarioOdontologoDto ToDto(this HorarioOdontologo entidad)
        {
            if (entidad == null) return null;

            // Obtener el nombre completo
            string nombreCompleto = "Desconocido";
            if (entidad.Odontologo?.Usuario != null)
            {
                nombreCompleto = $"{entidad.Odontologo.Usuario.Nombre} {entidad.Odontologo.Usuario.Apellidos}";
            }

            return new HorarioOdontologoDto
            {
                Id = entidad.Id,
                OdontologoId = entidad.Odontologo?.Id ?? 0,
                OdontologoNombre = nombreCompleto,
                DiaSemana = entidad.DiaSemana,
                HoraInicio = entidad.HoraInicio,
                HoraFin = entidad.HoraFin,
                Consultorio = entidad.Consultorio,
                Activo = entidad.Activo
            };
        }


        public static HorarioOdontologo MapFromDto(HorarioOdontologoDtoIn dto, HorarioOdontologo entidadExistente = null)
        {
            var entidad = entidadExistente ?? new HorarioOdontologo();

            entidad.DiaSemana = dto.DiaSemana;
            entidad.HoraInicio = dto.HoraInicio;
            entidad.HoraFin = dto.HoraFin;
            entidad.Consultorio = dto.Consultorio;
            entidad.Activo = dto.Activo;

            return entidad;
        }

        // ---- Recordatorio Mappings --- //

        public static RecordatorioDto ToDto(this Recordatorio entidad)
        {
            if (entidad == null) return null;

            return new RecordatorioDto
            {
                Id = entidad.Id,
                FechaEnvio = entidad.FechaEnvio,
                Mensaje = entidad.Mensaje,
                Enviado = entidad.Enviado,
                CitaId = entidad.CitaId,

                // Aplanado de datos (Flattening) con validación de nulos segura
                FechaCita = entidad.Cita != null ? entidad.Cita.FechaHora : DateTime.MinValue,

                DoctorNombre = entidad.Cita?.Odontologo?.Usuario != null
                    ? $"{entidad.Cita.Odontologo.Usuario.Nombre} {entidad.Cita.Odontologo.Usuario.Apellidos}"
                    : "Doctor no disponible",

                PacienteNombre = entidad.Cita?.Paciente?.Usuario != null
                    ? $"{entidad.Cita.Paciente.Usuario.Nombre} {entidad.Cita.Paciente.Usuario.Apellidos}"
                    : "Paciente no disponible"
            };
        }

        public static void MapFromDto(this Recordatorio entidad, RecordatorioDtoIn dto)
        {
            if (dto == null) return;

            entidad.CitaId = dto.CitaId;

            // Solo mapeamos el mensaje si viene personalizado.
            if (!string.IsNullOrEmpty(dto.MensajePersonalizado))
            {
                entidad.Mensaje = dto.MensajePersonalizado;
            }
        }

        // ---- Articulo Mappings --- //

        public static ArticuloDto ToDto(this Articulo entidad)
        {
            if (entidad == null) return null;

            return new ArticuloDto
            {
                Id = entidad.Id,
                Nombre = entidad.Nombre,
                Codigo = entidad.Codigo,
                Descripcion = entidad.Descripcion,
                Stock = entidad.Stock,
                Reutilizable = entidad.Reutilizable,
                Activo = entidad.Activo,
                Categoria = entidad.Categoria,
                CategoriaTexto = entidad.Categoria.ToString() // Conversión de Enum a String
            };
        }

        public static ArticuloDtoIn ToDtoIn(this Articulo entidad)
        {
            if (entidad == null) return null;

            return new ArticuloDtoIn
            {
                Id = entidad.Id,
                Nombre = entidad.Nombre,
                Descripcion = entidad.Descripcion,
                Codigo = entidad.Codigo,
                Categoria = entidad.Categoria,
                Reutilizable = entidad.Reutilizable,
                Stock = entidad.Stock,
                Activo = entidad.Activo
            };
        }

        public static void MapFromDto(this Articulo entidad, ArticuloDtoIn dto)
        {
            if (dto == null) return;

            entidad.Nombre = dto.Nombre;
            entidad.Descripcion = dto.Descripcion;
            entidad.Codigo = dto.Codigo;
            entidad.Categoria = dto.Categoria;
            entidad.Reutilizable = dto.Reutilizable;
            entidad.Stock = dto.Stock;
            entidad.Activo = dto.Activo;
        }

        // ---- CitaTratamiento Mappings --- //
        public static CitaTratamientoDto ToDto(this CitaTratamiento entidad)
        {
            if (entidad == null) return null;

            return new CitaTratamientoDto
            {
                Id = entidad.Id,
                TratamientoId = entidad.TratamientoId,
                TratamientoNombre = entidad.Tratamiento?.Nombre ?? "Tratamiento no disponible",
                CostoFinal = entidad.CostoFinal,
                Observaciones = entidad.Observaciones
            };
        }

        // ---- Cita Mappings --- //

        public static CitaDto ToDto(this Cita entidad)
        {
            if (entidad == null) return null;

            return new CitaDto
            {
                Id = entidad.Id,
                PacienteId = entidad.PacienteId,
                PacienteNombre = entidad.Paciente?.Usuario != null
                    ? $"{entidad.Paciente.Usuario.Nombre} {entidad.Paciente.Usuario.Apellidos}"
                    : "N/A",

                OdontologoId = entidad.OdontologoId,
                OdontologoNombre = entidad.Odontologo?.Usuario != null
                    ? $"{entidad.Odontologo.Usuario.Nombre} {entidad.Odontologo.Usuario.Apellidos}"
                    : "N/A",

                FechaHora = entidad.FechaHora,
                DuracionMinutos = entidad.DuracionMinutos,

                // Cálculo de fecha fin visual
                FechaFin = entidad.FechaHora.AddMinutes((int)entidad.DuracionMinutos),

                EstatusCita = entidad.EstatusCita,
                MotivoConsulta = entidad.MotivoConsulta,

                // Mapeo de la lista hija
                Tratamientos = entidad.CitasTratamientos?
                    .Select(ct => ct.ToDto())
                    .ToList() ?? new List<CitaTratamientoDto>(),

                // Propiedad calculada
                CostoTotal = entidad.CitasTratamientos?.Sum(ct => ct.CostoFinal) ?? 0
            };
        }

        public static void MapFromDto(this Cita entidad, CitaDtoIn dto)
        {
            if (dto == null) return;

            entidad.PacienteId = dto.PacienteId;
            entidad.OdontologoId = dto.OdontologoId;
            entidad.FechaHora = dto.FechaHora;
            entidad.DuracionMinutos = dto.DuracionMinutos;
            entidad.MotivoConsulta = dto.MotivoConsulta;
            entidad.EstatusCita = dto.EstatusCita;
        }

        // ---- Pago Mappings --- //
        public static PagoDto ToDto(this Pago entidad)
        {
            if (entidad == null) return null;

            return new PagoDto
            {
                Id = entidad.Id,
                Monto = entidad.Monto,
                FechaPago = entidad.FechaPago,
                MetodoPago = entidad.MetodoPago,
                CitaId = entidad.CitaId,

                // Navegación para obtener nombres
                PacienteNombre = entidad.Paciente?.Usuario != null
                    ? $"{entidad.Paciente.Usuario.Nombre} {entidad.Paciente.Usuario.Apellidos}"
                    : "N/A",

                OdontologoNombre = entidad.Cita?.Odontologo?.Usuario != null
                    ? $"{entidad.Cita.Odontologo.Usuario.Nombre} {entidad.Cita.Odontologo.Usuario.Apellidos}"
                    : "N/A"
            };
        }

        public static void MapFromDto(this Pago entidad, PagoDtoIn dto)
        {
            if (dto == null) return;

            entidad.CitaId = dto.CitaId;
            entidad.Monto = dto.Monto;
            entidad.MetodoPago = dto.MetodoPago;
            entidad.FechaPago = DateTime.Now;
        }

    }

}
