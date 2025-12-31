using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Administrador,Odontologo")]
    public class PacientesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PacientesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene una lista paginada de pacientes aplicando filtros avanzados.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<PacienteAdminDto>>> GetPacientes([FromQuery] PacienteFilterDto filtro)
        {
            // Llama a la lógica de negocio
            var pagedList = await _unitOfWork.Paciente.ObtenerListaPaginadaAsync(filtro);

            // Envuelve la respuesta en el DTO de paginación para la API
            var response = new PagedResultDto<PacienteAdminDto>
            {
                Items = pagedList, // La BL ya devuelve DTOs
                TotalCount = pagedList.TotalCount,
                TotalPages = pagedList.TotalPages,
                PageIndex = pagedList.PageIndex,
                HasNextPage = pagedList.HasNextPage,
                HasPreviousPage = pagedList.HasPreviousPage
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtiene los detalles de un paciente por su ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PacienteAdminDto>> GetPaciente(int id)
        {
            var paciente = await _unitOfWork.Paciente.ObtenerDetallePorIdAsync(id);
            if (paciente == null) return NotFound();

            return Ok(paciente);
        }

        /// <summary>
        /// Crea un nuevo expediente de paciente.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreatePaciente(PacienteAdminDtoIn dto)
        {
            try
            {
                await _unitOfWork.Paciente.CrearPacienteAdminAsync(dto);
                // Nota: Como CrearPacienteAdminAsync no devuelve el ID generado, 
                // retornamos Ok() o NoContent(). Si modificas la BL para devolver ID, 
                // podrías usar CreatedAtAction.
                return Ok(new { Mensaje = "Paciente creado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                // Captura errores de negocio (ej. "El usuario ya tiene paciente assigned")
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza la información médica de un paciente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaciente(int id, PacienteAdminDtoIn dto)
        {
            if (id != dto.Id) return BadRequest("El ID no coincide.");

            // Verificamos existencia antes de intentar actualizar
            var existe = await _unitOfWork.Paciente.ObtenerDetallePorIdAsync(id);
            if (existe == null) return NotFound();

            await _unitOfWork.Paciente.ActualizarPacienteAdminAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// Elimina el expediente de un paciente.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            await _unitOfWork.Paciente.EliminarPacienteAsync(id);
            return NoContent();
        }

        // --- ENDPOINT AUXILIAR PARA LA UI ---

        /// <summary>
        /// Obtiene la lista de usuarios que pueden ser asignados como nuevos pacientes.
        /// Filtra usuarios que ya tienen expediente (excepto el que se está editando).
        /// </summary>
        /// <param name="pacienteIdEdicion">Opcional: ID del paciente si se está editando.</param>
        [HttpGet("usuarios-disponibles")]
        public async Task<ActionResult<List<UsuarioDisponibleDto>>> GetUsuariosDisponibles(int? pacienteIdEdicion = null)
        {
            var usuarios = await _unitOfWork.Paciente.ObtenerUsuariosDisponiblesAsync(pacienteIdEdicion);
            return Ok(usuarios);
        }
    }
}
