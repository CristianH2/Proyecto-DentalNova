using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Protegemos todos los endpoints
    public class PagosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PagosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene el historial de pagos con filtros (Paciente, Fechas, Paginación).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Get([FromQuery] PagoFilterDto filtro)
        {
            var resultado = await _unitOfWork.Pago.ObtenerListaPaginadaAsync(filtro);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtiene el estado de cuenta de una cita específica.
        /// Útil para mostrar "Total a Pagar", "Pagado" y "Pendiente" antes de abonar.
        /// </summary>
        [HttpGet("estado-cuenta/{citaId}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetEstadoCuenta(int citaId)
        {
            try
            {
                var resultado = await _unitOfWork.Pago.ObtenerEstadoCuentaCitaAsync(citaId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Registra un nuevo pago.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Post([FromBody] PagoDtoIn dto)
        {
            try
            {
                var id = await _unitOfWork.Pago.RegistrarPagoAsync(dto);
                return Ok(new { id = id, mensaje = "Pago registrado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
