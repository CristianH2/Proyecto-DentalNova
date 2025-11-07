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
    [Authorize]
    public class PerfilController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PerfilController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --- USUARIO ---

        /// <summary>
        /// Actualiza la información de contacto del usuario autenticado.
        /// </summary>
        /// <param name="dto">Los datos a actualizar.</param>
        /// <returns>Los datos actualizados del usuario.</returns>
        [HttpPut("usuario")]
        [ProducesResponseType(typeof(UsuarioDto), 200)] // OK
        [ProducesResponseType(401)] // Unauthorized
        [ProducesResponseType(404)] // Not Found
        public async Task<IActionResult> ActualizarPerfilUsuario([FromBody] PerfilUsuarioDtoIn dto)
        {
            var usuarioId = ObtenerUsuarioIdDelToken();
            if (usuarioId == null)
            {
                return Unauthorized(new { Mensaje = "Token inválido." });
            }

            // Llama a la lógica de negocio
            var usuarioDto = await _unitOfWork.Usuario.ActualizarPerfilUsuarioAsync(usuarioId.Value, dto);

            if (usuarioDto == null)
            {
                return NotFound(new { Mensaje = "Usuario no encontrado." });
            }

            return Ok(usuarioDto);
        }

        // --- PACIENTE ---

        /// <summary>
        /// Crea o actualiza el perfil médico (Paciente) del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Esta operación es un "Upsert". Si el usuario no tiene perfil de paciente, lo crea.
        /// Si ya tiene, lo actualiza. La edad se calcula automáticamente desde la fecha de nacimiento.
        /// </remarks>
        /// <param name="dto">Los datos médicos del paciente.</param>
        /// <returns>El perfil del paciente creado o actualizado.</returns>
        [HttpPut("paciente")]
        [ProducesResponseType(typeof(PacienteDto), 200)] // OK
        [ProducesResponseType(401)] // Unauthorized
        [ProducesResponseType(500)] // Error (ej. si falta FechaNacimiento en el Usuario)
        public async Task<IActionResult> GuardarPerfilPaciente([FromBody] PerfilPacienteDtoIn dto)
        {
            var usuarioId = ObtenerUsuarioIdDelToken();
            if (usuarioId == null)
            {
                return Unauthorized(new { Mensaje = "Token inválido." });
            }

            try
            {
                // Llama a la lógica de negocio "Upsert"
                var pacienteDto = await _unitOfWork.Paciente.GuardarPerfilPacienteAsync(usuarioId.Value, dto);
                return Ok(pacienteDto);
            }
            catch (InvalidOperationException ex)
            {
                // Captura el error que lanzamos si el usuario no tiene FechaNacimiento
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la información de perfil del usuario autenticado.
        /// </summary>
        /// <returns>Los datos del perfil del usuario (Nombre, Email, CURP, Roles).</returns>
        [HttpGet("obtener-usuario")]
        [ProducesResponseType(typeof(UsuarioDto), 200)] // OK
        [ProducesResponseType(401)] // Unauthorized
        [ProducesResponseType(404)] // Not Found
        public async Task<IActionResult> ObtenerPerfilUsuario()
        {
            // Método para obtener el ID del token
            var usuarioId = ObtenerUsuarioIdDelToken();
            if (usuarioId == null)
            {
                return Unauthorized(new { Mensaje = "Token inválido." });
            }

            // Lógica de negocio
            var usuarioDto = await _unitOfWork.Usuario.ObtenerPerfilUsuarioAsync(usuarioId.Value);

            if (usuarioDto == null)
            {
                return NotFound(new { Mensaje = "Usuario no encontrado." });
            }

            return Ok(usuarioDto);
        }


        /// <summary>
        /// Obtiene el perfil médico (Paciente) del usuario autenticado.
        /// </summary>
        /// <returns>El perfil del paciente o un 404 si no existe.</returns>
        [HttpGet("obtener-paciente")]
        [ProducesResponseType(typeof(PacienteDto), 200)] // OK
        [ProducesResponseType(401)] // Unauthorized
        [ProducesResponseType(404)] // Not Found
        public async Task<IActionResult> ObtenerPerfilPaciente()
        {
            var usuarioId = ObtenerUsuarioIdDelToken(); // Reutiliza el método privado
            if (usuarioId == null)
            {
                return Unauthorized(new { Mensaje = "Token inválido." });
            }

            // Llama a la nueva lógica de negocio
            var pacienteDto = await _unitOfWork.Paciente.ObtenerPerfilPacienteAsync(usuarioId.Value);

            if (pacienteDto == null)
            {
                return NotFound(new { Mensaje = "Este usuario aún no tiene un perfil de paciente creado." });
            }

            return Ok(pacienteDto);
        }

        // --- MÉTODO PRIVADO ---

        /// <summary>
        /// Lee el ID del usuario directamente desde los claims del token JWT.
        /// </summary>
        private int? ObtenerUsuarioIdDelToken()
        {
            // Busca el claim "NameIdentifier" establecido en TokenService
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim != null && int.TryParse(idClaim.Value, out int usuarioId))
            {
                return usuarioId;
            }

            return null; // El token no es válido o no tiene el claim
        }
    }
}
