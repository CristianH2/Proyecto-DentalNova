using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Administrador")]
    public class TratamientosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TratamientosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene el catálogo de todos los tratamientos activos.
        /// </summary>
        /// <returns>Una lista de tratamientos con su Id, Nombre, Descripción y Costo.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TratamientoDto>), 200)] // 200 OK
        public async Task<IActionResult> ObtenerCatalogo()
        {
            var catalogo = await _unitOfWork.Tratamiento.ObtenerCatalogoAsync();

            return Ok(catalogo);
        }

        [HttpGet("admin")] // Ruta: /api/Tratamientos/admin (para diferenciar del catálogo simple)
        //[Authorize(Roles = "Administrador")]
        public async Task<ActionResult<PagedResultDto<TratamientoDto>>> GetTratamientosAdmin([FromQuery] TratamientoFilterDto filtro)
        {
            var pagedList = await _unitOfWork.Tratamiento.ObtenerListaPaginadaAsync(filtro);

            var response = new PagedResultDto<TratamientoDto>
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

        [HttpGet("admin/{id}")]
        //[Authorize(Roles = "Administrador")]
        public async Task<ActionResult<TratamientoDto>> GetTratamientoAdmin(int id)
        {
            var dto = await _unitOfWork.Tratamiento.ObtenerPorIdAdminAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public async Task<ActionResult> CreateTratamiento(TratamientoDtoIn dto)
        {
            try
            {
                await _unitOfWork.Tratamiento.CrearTratamientoAdminAsync(dto);
                return Ok(new { Mensaje = "Tratamiento creado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateTratamiento(int id, TratamientoDtoIn dto)
        {
            if (id != dto.Id) return BadRequest("El ID no coincide.");
            try
            {
                await _unitOfWork.Tratamiento.ActualizarTratamientoAdminAsync(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteTratamiento(int id)
        {
            await _unitOfWork.Tratamiento.EliminarTratamientoAsync(id);
            return NoContent();
        }
    }
}
