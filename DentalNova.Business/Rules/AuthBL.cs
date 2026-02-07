using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.Daos;
using DentalNova.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class AuthBL : IAuthBL
    {
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;

        public AuthBL(IRepository repository, ITokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var usuario = await _repository.Usuario.ObtenerPorEmailAsync(dto.Correo);

            if (usuario == null)
                throw new Exception("Credenciales incorrectas.");

            // Verificar Contraseña 
            bool passwordValido = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Password);

            if (!passwordValido)
                throw new Exception("Credenciales incorrectas.");

            // Validar estado de la cuenta
            if (!usuario.Activo)
                throw new Exception("Su cuenta se encuentra inactiva. Contacte al administrador.");

            var tokenJwt = _tokenService.GenerarToken(usuario);

            var response = new LoginResponseDto
            {
                UsuarioId = usuario.Id,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellidos}",
                Token = tokenJwt,
                Roles = usuario.Roles.Select(r => r.Nombre).ToList()
            };

            // Buscar IDs vinculados para redirección inteligente
            if (response.Roles.Contains("Paciente"))
            {
                var paciente = await _repository.Paciente.ObtenerPorUsuarioIdAsync(usuario.Id);
                if (paciente != null) response.PacienteId = paciente.Id;
            }

            if (response.Roles.Contains("Odontologo"))
            {
                var odontologo = await _repository.Odontologo.ObtenerPorUsuarioIdAsync(usuario.Id);
                if (odontologo != null) response.OdontologoId = odontologo.Id;
            }

            return response;
        }

    }
}
