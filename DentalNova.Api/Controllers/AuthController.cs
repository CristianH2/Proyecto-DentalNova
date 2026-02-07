using DentalNova.Business.Helpers;
using DentalNova.Business.Rules;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Endpoint para el login de usuarios.
        /// <param name="loginDto">Objeto que contiene el correo y la contraseña del usuario
        /// </summary>

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var response = await _unitOfWork.Auth.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // 401 Unauthorized
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint PÚBLICO para registro completo desde la App Móvil.
        /// Crea Usuario y Paciente en una sola transacción lógica.
        /// </summary>
        [HttpPost("RegistroCompleto")]
        public async Task<IActionResult> RegistroCompleto([FromBody] RegistroCompletoDto dto)
        {
            // Validaciones
            if (await _unitOfWork.Usuario.EmailYaExisteAsync(dto.Usuario.CorreoElectronico))
                return BadRequest(new { Mensaje = "El correo electrónico ya está registrado." });

            if (!string.IsNullOrEmpty(dto.Usuario.CURP) && await _unitOfWork.Usuario.CurpYaExisteAsync(dto.Usuario.CURP))
                return BadRequest(new { Mensaje = "La CURP ya está registrada." });

            try
            {
                // Crea usuario
                UsuarioDto nuevoUsuarioDto = await _unitOfWork.Usuario.RegistrarAsync(dto.Usuario);

                // Crea perfil de paciente asociado
                await _unitOfWork.Paciente.GuardarPerfilPacienteAsync(nuevoUsuarioDto.Id, dto.Paciente);
                return Ok(new { Mensaje = "Registro completado exitosamente. Ya puede iniciar sesión." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = "Error al registrar: " + ex.Message });
            }
        }
    }
}
