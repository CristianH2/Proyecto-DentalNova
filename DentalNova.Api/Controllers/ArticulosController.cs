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
    [Authorize]
    public class ArticulosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        // Inyectamos la "Unidad de Trabajo" principal
        public ArticulosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene el catálogo de todos los artículos activos en inventario.
        /// </summary>
        /// <returns>Una lista de artículos con sus detalles.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticuloDto>), 200)] // 200 OK
        [ProducesResponseType(401)] // 401 Unauthorized
        public async Task<IActionResult> ObtenerCatalogo()
        {
            // 1. Llama a la capa de lógica de negocio (ArticuloBL)
            var catalogo = await _unitOfWork.Articulo.ObtenerCatalogoAsync();

            // 2. Devuelve los DTOs como JSON con un código 200 OK
            return Ok(catalogo);
        }
    }
}
