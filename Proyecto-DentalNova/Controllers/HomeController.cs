using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_DentalNova.Models;
using System.Diagnostics;

namespace Proyecto_DentalNova.Controllers
{
    [Authorize(Roles = "Administrador, Odontologo")]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            DashboardDto modelo;

            try
            {
                modelo = await _dashboardService.ObtenerResumenAsync();
            }
            catch
            {
                modelo = new DashboardDto();
                TempData["MensajeError"] = "No se pudo conectar con el servidor de datos.";
            }

            // Si es Odontólogo, mostramos su vista específica
            if (User.IsInRole("Odontologo"))
            {
                return View("IndexOdontologo", modelo);
            }

            return View(modelo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
