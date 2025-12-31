using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Administrador")]
    public class OdontologosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OdontologosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene una lista paginada de odontólogos con filtros (incluyendo especialidad).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<OdontologoDto>>> GetOdontologos([FromQuery] OdontologoFilterDto filtro)
        {
            // Llama a la lógica de negocio
            var pagedList = await _unitOfWork.Odontologo.ObtenerListaPaginadaAsync(filtro);

            // Envuelve en el DTO de respuesta paginada
            var response = new PagedResultDto<OdontologoDto>
            {
                Items = pagedList,
                TotalCount = pagedList.TotalCount,
                TotalPages = pagedList.TotalPages,
                PageIndex = pagedList.PageIndex,
                HasNextPage = pagedList.HasNextPage,
                HasPreviousPage = pagedList.HasPreviousPage
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtiene los detalles de un odontólogo por su ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OdontologoDto>> GetOdontologo(int id)
        {
            var odontologo = await _unitOfWork.Odontologo.ObtenerPorIdAdminAsync(id);
            if (odontologo == null) return NotFound();

            return Ok(odontologo);
        }

        /// <summary>
        /// Registra un nuevo odontólogo y asigna sus especialidades.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateOdontologo(OdontologoDtoIn dto)
        {
            try
            {
                await _unitOfWork.Odontologo.CrearOdontologoAdminAsync(dto);
                return Ok(new { Mensaje = "Odontólogo registrado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                // Captura validaciones de negocio
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza la información y especialidades de un odontólogo.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOdontologo(int id, OdontologoDtoIn dto)
        {
            if (id != dto.Id) return BadRequest("El ID no coincide.");

            var existe = await _unitOfWork.Odontologo.ObtenerPorIdAdminAsync(id);
            if (existe == null) return NotFound();

            await _unitOfWork.Odontologo.ActualizarOdontologoAdminAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// Elimina un odontólogo del sistema.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOdontologo(int id)
        {
            await _unitOfWork.Odontologo.EliminarOdontologoAsync(id);
            return NoContent();
        }

        // --- ENDPOINTS AUXILIARES PARA LA UI ---

        /// <summary>
        /// Obtiene usuarios candidatos para ser odontólogos.
        /// </summary>
        [HttpGet("usuarios-disponibles")]
        public async Task<ActionResult<List<UsuarioDisponibleDto>>> GetUsuariosDisponibles(int? odontologoIdEdicion = null)
        {
            var usuarios = await _unitOfWork.Odontologo.ObtenerUsuariosDisponiblesAsync(odontologoIdEdicion);
            return Ok(usuarios);
        }

        /// <summary>
        /// Obtiene el catálogo completo de especialidades.
        /// </summary>
        [HttpGet("especialidades")]
        public async Task<ActionResult<List<EspecialidadDto>>> GetEspecialidades()
        {
            var especialidades = await _unitOfWork.Odontologo.ObtenerTodasEspecialidadesAsync();
            return Ok(especialidades);
        }
    }
}
