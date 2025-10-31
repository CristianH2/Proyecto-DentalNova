using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InicioSesionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public InicioSesionController(IUnitOfWork unitOfWork)
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
    }
}
