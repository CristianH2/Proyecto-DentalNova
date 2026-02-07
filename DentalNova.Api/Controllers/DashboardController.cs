using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Odontologo")]
        public async Task<ActionResult<DashboardDto>> ObtenerResumen()
        {
            try
            {
                int? usuarioId = null;

                if (User.IsInRole("Odontologo"))
                {
                    // Buscamos el Claim  que guardamos al hacer Login
                    var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (claimId != null && int.TryParse(claimId.Value, out int id))
                    {
                        usuarioId = id;
                    }
                }

                var resultado = await _unitOfWork.Dashboard.ObtenerResumenAsync(usuarioId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al generar el dashboard: " + ex.Message });
            }
        }
    }
}
