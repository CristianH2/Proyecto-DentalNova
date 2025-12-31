using BCrypt.Net;
using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
//using DentalNova.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class UsuarioBL : IUsuarioBL
    {
        private readonly IRepository _repositorio;
        //private readonly ITokenService _tokenService;

        public UsuarioBL(IRepository repositorio) //ITokenService tokenService
        {
            _repositorio = repositorio;
            //_tokenService = tokenService;
        }

        public async Task<UsuarioDto> ActualizarPerfilUsuarioAsync(int usuarioId, PerfilUsuarioDtoIn dto)
        {
            // Obtener el usuario por el ID (que viene del token)
            var usuario = await _repositorio.Usuario.ObtenerPorIdAsync(usuarioId);
            if (usuario == null)
            {
                return null; // No se encontró el usuario
            }

            // Actualizar solo los campos permitidos del DTO
            usuario.Telefono = dto.Telefono;
            usuario.Genero = dto.Genero;

            // Guardar los cambios en la base de datos
            await _repositorio.Usuario.ActualizarAsync(usuario);

            // Devolver el DTO de salida actualizado
            return usuario.ToDto();
        }

        public async Task<bool> CambiarPasswordAsync(int usuarioId, CambioPasswordDtoIn cambioDto)
        {
            // Obtener el usuario por el ID (que viene del token)
            var usuario = await _repositorio.Usuario.ObtenerPorIdAsync(usuarioId);
            if (usuario == null)
            {
                return false; // No debería pasar si el token es válido
            }

            // Verificar la contraseña ACTUAL
            bool esPasswordActualValido = BCrypt.Net.BCrypt.Verify(cambioDto.PasswordActual, usuario.Password);

            if (!esPasswordActualValido)
            {
                return false; // La contraseña actual no coincide
            }

            var nuevoHash = BCrypt.Net.BCrypt.HashPassword(cambioDto.PasswordNueva);

            // Guardar el nuevo hash en la base de datos
            return await _repositorio.Usuario.ActualizarPasswordAsync(usuarioId, nuevoHash);
        }

        public async Task<Usuario> ValidarCredencialesAsync(InicioDeSesionDto inicioDeSesion)
        {
            // Buscar al usuario (incluyendo roles para el token/cookie)
            var usuario = await _repositorio.Usuario.ObtenerPorEmailAsync(inicioDeSesion.Correo);

            if (usuario == null)
            {
                return null; // Usuario no existe
            }

            // 2. Validar contraseña
            bool esPasswordValido = BCrypt.Net.BCrypt.Verify(inicioDeSesion.Password, usuario.Password);

            if (!esPasswordValido)
            {
                return null; // Contraseña incorrecta
            }

            // Devolver la entidad Usuario completa
            return usuario;
        }

        public async Task<UsuarioDto> ObtenerPerfilUsuarioAsync(int usuarioId)
        {
            // Obtener el usuario por el ID (que viene del token)
            var usuario = await _repositorio.Usuario.ObtenerPorIdAsync(usuarioId);

            if (usuario == null)
            {
                return null; // No se encontró el usuario
            }

            // Mapear la entidad al DTO de salida
            return usuario.ToDto();
        }

        public async Task<UsuarioDto> RegistrarAsync(UsuarioDtoIn usuarioDtoIn)
        {
            // Verificar si el correo ya existe
            var usuarioExistente = await _repositorio.Usuario.ObtenerPorEmailAsync(usuarioDtoIn.CorreoElectronico);
            if (usuarioExistente != null)
            {
                return null; // No se puede registrar
            }

            // Mapear DTO a Entidad 
            var nuevoUsuario = usuarioDtoIn.ToEntidad();

            // Agregar el nuevo usuario a la BD
            var usuarioCreado = await _repositorio.Usuario.AgregarAsync(nuevoUsuario);

            // Crear y asignar el rol por defecto ("paciente")
            var nuevoRol = new Rol
            {
                Nombre = "Paciente",
                Descripcion = "Rol asignado automáticamente por API",
                Usuario = usuarioCreado
            };

            // Agregar el nuevo rol a la BD
            await _repositorio.Rol.AgregarAsync(nuevoRol);
            usuarioCreado.Roles.Add(nuevoRol);

            return usuarioCreado.ToDto();
        }

        // --- Métodos para Admin MVC ---


        // Obtiene la lista paginada de usuarios aplicando filtros.
        // (Mueve la lógica de UsuarioController.Index)

        public async Task<PaginatedList<Usuario>> ObtenerListaPaginadaAsync(UsuarioFilterDto filtro)
        {
            // Obtiene la consulta base del repositorio
            IQueryable<Usuario> query = _repositorio.Usuario.ObtenerQueryableParaFiltro();

            // 2. Aplica los filtros del DTO
            if (filtro.Id.HasValue)
                query = query.Where(u => u.Id == filtro.Id.Value);
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike))
                query = query.Where(u => u.Nombre.Contains(filtro.NombreLike));
            if (!string.IsNullOrWhiteSpace(filtro.ApellidosLike))
                query = query.Where(u => u.Apellidos.Contains(filtro.ApellidosLike));
            if (!string.IsNullOrWhiteSpace(filtro.CorreoLike))
                query = query.Where(u => u.CorreoElectronico.Contains(filtro.CorreoLike));
            if (!string.IsNullOrWhiteSpace(filtro.TelefonoLike))
                query = query.Where(u => u.Telefono.Contains(filtro.TelefonoLike));
            if (filtro.Genero.HasValue)
                query = query.Where(u => u.Genero == filtro.Genero.Value);
            if (filtro.Activo.HasValue)
                query = query.Where(u => u.Activo == filtro.Activo.Value);

            // Aplica ordenamiento
            //query = query.OrderBy(u => u.Apellidos).ThenBy(u => u.Nombre); 
            query = query.OrderBy(u => u.Id);

            // Ejecuta la paginación
            return await PaginatedList<Usuario>.CreateAsync(query, filtro.Page, filtro.PageSize);
        }

        // Obtiene un usuario por ID (para Details o Edit).
        public async Task<Usuario> ObtenerPorIdAdminAsync(int id)
        {
            // (Usamos el 'ObtenerPorIdAsync' que ya incluye roles)
            return await _repositorio.Usuario.ObtenerPorIdAsync(id);
        }

        // Obtiene la fecha de nacimiento formateada para JSON.
        // (Mueve la lógica de UsuarioController.GetUsuarioFechaNacimiento)
        public async Task<string> ObtenerFechaNacimientoJsonAsync(int id)
        {
            var usuario = await _repositorio.Usuario.ObtenerPorIdAsync(id);
            if (usuario == null || !usuario.FechaNacimiento.HasValue)
            {
                return null;
            }
            return usuario.FechaNacimiento.Value.ToString("yyyy-MM-dd");
        }

        // Crea un nuevo usuario (admin).
        // (Mueve la lógica de UsuarioController.Create POST)
        public async Task CrearUsuarioAdminAsync(Usuario usuario, string newPassword, List<string> rolesSeleccionados)
        {
            // Hashea la contraseña
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Esto guarda y obtiene el ID
            var usuarioCreado = await _repositorio.Usuario.AgregarAsync(usuario);

            // Asigna los roles seleccionados
            foreach (var nombreRol in rolesSeleccionados)
            {
                var nuevoRol = new Rol
                {
                    Nombre = nombreRol,
                    Descripcion = "Asignado desde panel de admin",
                    Usuario = usuarioCreado // Asigna la entidad
                };
                // Esto llama a SaveChanges() por cada rol
                await _repositorio.Rol.AgregarAsync(nuevoRol);
            }
        }

        // Actualiza un usuario (admin).
        // (Mueve la lógica de UsuarioController.Edit POST)
        public async Task ActualizarUsuarioAdminAsync(Usuario usuario, string? newPassword, List<string> rolesSeleccionados)
        {
            bool actualizarPassword = !string.IsNullOrEmpty(newPassword);

            if (actualizarPassword)
            {
                // Hashea la nueva contraseña si se proporcionó
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            // Actualiza la entidad Usuario (esto llama a SaveChanges())
            await _repositorio.Usuario.ActualizarUsuarioAdminAsync(usuario, actualizarPassword);

            // Borra todos los roles antiguos de este usuario (llama a SaveChanges())
            await _repositorio.Rol.EliminarPorUsuarioIdAsync(usuario.Id);

            // Re-agrega los nuevos roles seleccionados (cada uno llama a SaveChanges())
            foreach (var nombreRol in rolesSeleccionados)
            {
                var nuevoRol = new Rol
                {
                    Nombre = nombreRol,
                    Descripcion = "Actualizado desde panel de admin",
                    Usuario = usuario // Asigna la entidad existente
                };
                await _repositorio.Rol.AgregarAsync(nuevoRol);
            }
        }

        // Elimina un usuario (admin).
        // (Mueve la lógica de UsuarioController.DeleteConfirmed)
        public async Task EliminarUsuarioAsync(int id)
        {
            await _repositorio.Usuario.EliminarAsync(id);
        }

        // --- MÉTODOS DE VALIDACIÓN (movidos del controlador) ---

        public async Task<bool> EmailYaExisteAsync(string email, int? usuarioId = null)
        {
            return await _repositorio.Usuario.EmailYaExisteAsync(email, usuarioId);
        }

        public async Task<bool> CurpYaExisteAsync(string curp, int? usuarioId = null)
        {
            return await _repositorio.Usuario.CurpYaExisteAsync(curp, usuarioId);
        }
    }
}
