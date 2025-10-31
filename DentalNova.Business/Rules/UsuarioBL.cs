using BCrypt.Net;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
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
                Expiracion = DateTime.UtcNow.AddMinutes(20)
            };
        }
    }
}
