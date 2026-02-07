using DentalNova.Business.Rules;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ArticulosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene el catálogo paginado y filtrado de artículos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<ArticuloDto>), 200)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Get([FromQuery] ArticuloFilterDto filtro)
        {
            // Accedemos a la BL a través del UnitOfWork
            var resultado = await _unitOfWork.Articulo.ObtenerListaPaginadaAsync(filtro);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtiene un artículo por ID para edición.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ArticuloDtoIn), 200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var resultado = await _unitOfWork.Articulo.ObtenerParaEditarAsync(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Crea un nuevo artículo en el inventario.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Post([FromBody] ArticuloDtoIn dto)
        {
            try
            {
                var id = await _unitOfWork.Articulo.CrearAsync(dto);
                // Retorna 201 Created y la url para consultar el recurso creado
                return CreatedAtAction(nameof(Get), new { id = id }, new { id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza la información de un artículo.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Put(int id, [FromBody] ArticuloDtoIn dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la URL no coincide con el cuerpo de la petición.");
            }

            try
            {
                await _unitOfWork.Articulo.ActualizarAsync(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Realiza un borrado lógico (Soft Delete).
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _unitOfWork.Articulo.EliminarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Alterna el estatus Activo/Inactivo.
        /// </summary>
        [HttpPost("{id}/estatus")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstatus(int id)
        {
            try
            {
                await _unitOfWork.Articulo.CambiarEstatusAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
