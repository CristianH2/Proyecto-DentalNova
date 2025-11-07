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
    public class CitasController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CitasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Agenda una nueva cita para el paciente autenticado.
        /// </summary>
        /// <remarks>
        /// El paciente solo envía la fecha/hora y motivo.
        /// El sistema asignará automáticamente el tratamiento de "Diagnóstico"
        /// y buscará un odontólogo disponible.
        /// </remarks>
        /// <param name="dto">La fecha, hora y motivo de la consulta.</param>
        /// <returns>La cita confirmada o un error si no hay disponibilidad.</returns>
        [HttpPost("Agendar-Cita")]
        [ProducesResponseType(typeof(CitaAgendadaDto), 201)] // 201 Created
        [ProducesResponseType(typeof(object), 400)] // 400 Bad Request
        [ProducesResponseType(401)] // 401 Unauthorized
        public async Task<IActionResult> AgendarCita([FromBody] CitaDtoIn dto)
        {
            var usuarioId = ObtenerUsuarioIdDelToken();
            if (usuarioId == null)
            {
                return Unauthorized(new { Mensaje = "Token inválido." });
            }

            try
            {
                // Llama a la lógica de negocio (CitaBL)
                var citaConfirmada = await _unitOfWork.Cita.AgendarCitaPacienteAsync(usuarioId.Value, dto);

                // Devuelve 201 Created con la cita confirmada
                return CreatedAtAction(nameof(AgendarCita), new { id = citaConfirmada.Id }, citaConfirmada);
            }
            catch (InvalidOperationException ex) // Captura los errores de negocio (ej. "No hay odontólogos")
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex) // Captura cualquier otro error inesperado
            {
                return StatusCode(500, new { Mensaje = "Ocurrió un error inesperado al agendar la cita.", Detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el historial de todas las citas del paciente autenticado.
        /// </summary>
        /// <returns>Una lista del historial de citas.</returns>
        [HttpGet("Historial-Citas")]
        [ProducesResponseType(typeof(IEnumerable<HistorialCitaDto>), 200)] // 200 OK
        [ProducesResponseType(401)] // Unauthorized
        public async Task<IActionResult> ObtenerHistorial()
        {
            // Método para obtener el ID del token
            var usuarioId = ObtenerUsuarioIdDelToken();
            if (usuarioId == null)
            {
                return Unauthorized(new { Mensaje = "Token inválido." });
            }
            var historial = await _unitOfWork.Cita.ObtenerHistorialPacienteAsync(usuarioId.Value);

            // Devuelve la lista (estará vacía si no tiene historial)
            return Ok(historial);
        }

        /// <summary>
        /// Cancela una cita programada del paciente autenticado.
        /// </summary>
        /// <param name="citaId">El ID de la cita que se desea cancelar.</param>
        /// <returns>Un mensaje de éxito o error.</returns>
        [HttpPut("{citaId:int}/cancelar")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(typeof(object), 400)] // Bad Request (ej. ya estaba cancelada)
        [ProducesResponseType(401)] // Unauthorized (token inválido)
        [ProducesResponseType(403)] // Forbidden (no es su cita)
        [ProducesResponseType(404)] // Not Found (cita no existe)
        public async Task<IActionResult> CancelarCita(int citaId)
        {
            var usuarioId = ObtenerUsuarioIdDelToken();
            if (usuarioId == null)
            {
                return Unauthorized(new { Mensaje = "Token inválido." });
            }

            try
            {
                var exito = await _unitOfWork.Cita.CancelarCitaAsync(usuarioId.Value, citaId);

                if (exito)
                {
                    return Ok(new { Mensaje = "Cita cancelada exitosamente." });
                }

                // Esto solo ocurriría si SaveChangesAsync() > 0 devuelve false
                return BadRequest(new { Mensaje = "No se pudo actualizar la cita en la base de datos." });
            }
            catch (KeyNotFoundException ex)
            {
                // Cita no encontrada
                return NotFound(new { Mensaje = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                // No es su cita
                return StatusCode(403, new { Mensaje = ex.Message }); // 403 Forbidden
            }
            catch (InvalidOperationException ex)
            {
                // Cita ya completada o paciente no existe
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                // Error inesperado
                return StatusCode(500, new { Mensaje = "Ocurrió un error inesperado.", Detalle = ex.Message });
            }
        }

        // --- MÉTODO PRIVADO ---

        /// <summary>
        /// Lee el ID del usuario directamente desde los claims del token JWT.
        /// </summary>
        private int? ObtenerUsuarioIdDelToken()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim != null && int.TryParse(idClaim.Value, out int usuarioId))
            {
                return usuarioId;
            }

            return null; // El token no es válido o no tiene el claim
        }
    }
}
