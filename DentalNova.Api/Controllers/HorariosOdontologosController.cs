using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosOdontologosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public HorariosOdontologosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene la lista de horarios configurados para un odontólogo específico.
        /// Equivalente al catálogo, pero filtrado por doctor.
        /// </summary>
        /// <param name="odontologoId">El ID del odontólogo.</param>
        /// <returns>Lista de horarios formateados.</returns>
        [HttpGet("odontologo/{odontologoId}")]
        [ProducesResponseType(typeof(IEnumerable<HorarioOdontologoDto>), 200)]
        [Authorize(Roles = "Administrador,Odontologo,Paciente")]
        public async Task<IActionResult> ObtenerPorOdontologo(int odontologoId)
        {
            var lista = await _unitOfWork.HorarioOdontologo.ObtenerPorOdontologoAsync(odontologoId);
            return Ok(lista);
        }

        /// <summary>
        /// Obtiene un horario específico por su ID (para edición).
        /// </summary>
        /// <param name="id">ID del horario.</param>
        /// <returns>El DTO del horario.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HorarioOdontologoDto), 200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Administrador,Odontologo")]
        public async Task<ActionResult<HorarioOdontologoDto>> GetHorarioAdmin(int id)
        {
            var dto = await _unitOfWork.HorarioOdontologo.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Crea un nuevo horario para un odontólogo.
        /// </summary>
        /// <param name="dto">Datos del horario.</param>
        /// <returns>Mensaje de éxito.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> CreateHorario(HorarioOdontologoDtoIn dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // GuardarAsync en tu BL maneja la creación si ID = 0
                await _unitOfWork.HorarioOdontologo.GuardarAsync(dto);
                return Ok(new { Mensaje = "Horario creado exitosamente." });
            }
            catch (Exception ex)
            {
                // Capturamos excepciones de negocio (ej. solapamiento)
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un horario existente.
        /// </summary>
        /// <param name="id">ID del horario a actualizar.</param>
        /// <param name="dto">Datos actualizados.</param>
        /// <returns>NoContent si es exitoso.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateHorario(int id, HorarioOdontologoDtoIn dto)
        {
            if (id != dto.Id) return BadRequest(new { Mensaje = "El ID de la URL no coincide con el cuerpo de la petición." });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // GuardarAsync en tu BL maneja la actualización si ID > 0
                await _unitOfWork.HorarioOdontologo.GuardarAsync(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Elimina (o desactiva) un horario.
        /// </summary>
        /// <param name="id">ID del horario.</param>
        /// <returns>NoContent si es exitoso.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteHorario(int id)
        {
            try
            {
                await _unitOfWork.HorarioOdontologo.EliminarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }
}
