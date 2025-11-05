using BCrypt.Net;
using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class UsuarioBL : IUsuarioBL
    {
        private readonly IRepositoriy _repositorio;
        private readonly ITokenService _tokenService;

        public UsuarioBL(IRepositoriy repositorio, ITokenService tokenService)
        {
            _repositorio = repositorio;
            _tokenService = tokenService;
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

        public async Task<TokenDto> LoginAsync(InicioDeSesionDto inicioDeSesion)
        {
            // Buscar al usuario por email
            var usuario = await _repositorio.Usuario.ObtenerPorEmailAsync(inicioDeSesion.Correo);

            // Validar que el usuario exista
            if (usuario == null)
            {
                return null; // Credenciales inválidas
            }

            // Validar la contraseña
            bool esPasswordValido = BCrypt.Net.BCrypt.Verify(inicioDeSesion.Password, usuario.Password);

            if (!esPasswordValido)
            {
                return null; // Credenciales inválidas
            }

            // Generar el token
            var tokenString = _tokenService.GenerarToken(usuario);

            // Devolver el DTO
            return new TokenDto
            {
                Token = tokenString,
                Expiracion = DateTime.Now.AddMinutes(20)
            };
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

            // Generar token para auto-login
            var tokenString = _tokenService.GenerarToken(usuarioCreado);

            return usuarioCreado.ToDto();
        }
    }
}
