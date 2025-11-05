using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        /// Inicia sesión de un usuario y genera un token JWT.
        /// </summary>
        /// <param name="inicioDeSesionDto">Las credenciales (Correo y Password).</param>
        /// <returns>Un TokenDto si las credenciales son válidas.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenDto), 200)] // 200 OK
        [ProducesResponseType(401)] // 401 Unauthorized
        public async Task<IActionResult> Login(InicioDeSesionDto inicioDeSesionDto)
        {
            var tokenDto = await _unitOfWork.Usuario.LoginAsync(inicioDeSesionDto);

            // Comprueba si el login falló
            if (tokenDto == null)
            {
                // Devuelve 401 Unauthorized.
                return Unauthorized(new { Mensaje = "Credenciales inválidas." });
            }

            // Si el login fue exitoso, devuelve el token
            return Ok(tokenDto);
        }

        /// <summary>
        /// Registra un nuevo usuario (paciente) en el sistema.
        /// </summary>
        /// <param name="usuarioDtoIn">Los datos del nuevo usuario.</param>
        /// <returns>Los datos del usuario recién creado.</returns>
        [HttpPost("registrar")]
        [AllowAnonymous] // Cualquiera puede registrarse
        [ProducesResponseType(typeof(UsuarioDto), 201)] // 201 Created
        [ProducesResponseType(400)] // 400 Bad Request (si el correo ya existe)
        public async Task<IActionResult> Registrar(UsuarioDtoIn usuarioDtoIn)
        {
            var usuarioDto = await _unitOfWork.Usuario.RegistrarAsync(usuarioDtoIn);

            // Comprueba si el registro falló
            if (usuarioDto == null)
            {
                // Devolvemos 400 Bad Request.
                return BadRequest(new { Mensaje = "El correo electrónico ya está en uso." });
            }

            // Si el registro fue exitoso, devuelve 201 Created
            return CreatedAtAction(nameof(Login), usuarioDto);
        }


        /// <summary>
        /// Cambia la contraseña de un usuario autenticado.
        /// </summary>
        /// <param name="cambioDtoIn">La contraseña actual y la nueva contraseña.</param>
        /// <returns>Un mensaje de éxito o error.</returns>
        [HttpPost("cambiar-password")]
        [Authorize] // Solo usuarios logueados
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request (contraseña actual incorrecta)
        [ProducesResponseType(401)] // Unauthorized (token no válido)
        public async Task<IActionResult> CambiarPassword(CambioPasswordDtoIn cambioDtoIn)
        {
            // Obtener el ID del Usuario desde el Token
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim == null || !int.TryParse(idClaim.Value, out int usuarioId))
            {
                return Unauthorized(new { Mensaje = "Token inválido o no contiene ID de usuario." });
            }

            // Aplicar reglas de negocio
            var exito = await _unitOfWork.Usuario.CambiarPasswordAsync(usuarioId, cambioDtoIn);

            // Devolver la respuesta
            if (!exito)
            {
                return BadRequest(new { Mensaje = "La contraseña actual es incorrecta." });
            }

            return Ok(new { Mensaje = "Contraseña cambiada exitosamente." });
        }
    }
}
