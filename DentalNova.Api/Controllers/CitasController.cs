using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CitasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene una cita específica por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CitaDto), 200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Administrador, Odontologo")]
        public async Task<IActionResult> Get(int id)
        {
            var cita = await _unitOfWork.Cita.ObtenerPorIdAsync(id);
            if (cita == null) return NotFound();
            return Ok(cita);
        }

        /// <summary>
        /// Obtiene un listado paginado y filtrado de citas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<CitaDto>), 200)]
        [Authorize(Roles = "Administrador,Odontologo,Paciente")]
        public async Task<IActionResult> GetList([FromQuery] CitaFilterDto filtro, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _unitOfWork.Cita.ObtenerListaPaginadaAsync(filtro, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Crea una nueva cita y sus tratamientos iniciales.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador,Odontologo,Paciente")]
        public async Task<IActionResult> Post([FromBody] CitaDtoIn dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var id = await _unitOfWork.Cita.CrearAsync(dto);
                return Ok(new { Mensaje = "Cita agendada exitosamente.", Id = id });
            }
            catch (Exception ex)
            {
                // Capturamos errores de validación de negocio (Horario ocupado, fuera de turno, etc.)
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos generales de una cita (Fecha, Hora, Motivo).
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador, Odontologo")]
        public async Task<IActionResult> Put(int id, [FromBody] CitaDtoIn dto)
        {
            if (id != dto.Id) return BadRequest(new { Mensaje = "El ID de la URL no coincide con el cuerpo." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _unitOfWork.Cita.ActualizarAsync(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estatus de una cita (Ej. Cancelar, Completar).
        /// </summary>
        [HttpPatch("{id}/estatus")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador,Odontologo,Paciente")]
        public async Task<IActionResult> PatchEstatus(int id, [FromBody] EstatusCita nuevoEstatus)
        {
            try
            {
                await _unitOfWork.Cita.CambiarEstatusAsync(id, nuevoEstatus);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // NoContent
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(400)] // BadRequest
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Esto llamará a tu repositorio que hace el "Soft Delete" (cambia a Cancelada)
                await _unitOfWork.Cita.EliminarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }
}
