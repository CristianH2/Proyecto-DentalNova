using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [ProducesResponseType(typeof(IEnumerable<DentalNova.Core.Dtos.TratamientoDto>), 200)] // 200 OK
        public async Task<IActionResult> ObtenerCatalogo()
        {
            var catalogo = await _unitOfWork.Tratamiento.ObtenerCatalogoAsync();

            return Ok(catalogo);
        }
    }
}
