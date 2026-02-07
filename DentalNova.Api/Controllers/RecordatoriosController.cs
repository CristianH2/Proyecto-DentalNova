using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordatoriosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecordatoriosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Genera y envía un recordatorio automático para una cita específica.
        /// Acción realizada por el Administrador o Doctor.
        /// </summary>
        [HttpPost("enviar/{citaId}")]
        [Authorize(Roles = "Administrador,Odontologo")]
        public async Task<IActionResult> EnviarRecordatorio(int citaId)
        {
            try
            {
                // Llamamos a la BL
                await _unitOfWork.Recordatorio.EnviarRecordatorioManualAsync(citaId);

                return Ok(new { message = "Recordatorio enviado y registrado correctamente." });
            }
            catch (Exception ex)
            {
                // Capturamos errores de negocio
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el historial de mensajes (buzón) de un paciente.
        /// </summary>
        [HttpGet("mis-mensajes/{pacienteId}")]
        [Authorize(Roles = "Administrador,Paciente")]
        public async Task<IActionResult> ObtenerMensajes(int pacienteId)
        {
            try
            {
                var mensajes = await _unitOfWork.Recordatorio.ObtenerBuzonPacienteAsync(pacienteId);
                return Ok(mensajes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al recuperar mensajes: " + ex.Message });
            }
        }
    }
}
