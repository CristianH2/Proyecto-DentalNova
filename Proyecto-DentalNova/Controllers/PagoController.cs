using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_DentalNova.Models.PagoViewModel;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace Proyecto_DentalNova.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PagoController : Controller
    {
        private readonly IPagoService _pagoService;

        public PagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        // Helper
        private IEnumerable<SelectListItem> ObtenerMetodosPagoList()
        {
            return Enum.GetValues(typeof(MetodoPago)).Cast<MetodoPago>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                });
        }

        // GET: Pagos
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] PagoFilterDto filtro)
        {
            // Valores por defecto
            if (filtro.Page < 1) filtro.Page = 1;
            if (filtro.PageSize < 1) filtro.PageSize = 10;

            // Llamada al API
            var apiResult = await _pagoService.ObtenerListaPaginadaAsync(filtro);

            var listaPaginada = PaginatedList<PagoDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                filtro.Page,
                filtro.PageSize
            );

            var vm = new PagoIndexViewModel
            {
                Filtro = filtro,
                Resultados = listaPaginada
            };

            return View(vm);
        }

        // GET: Pagos/Create?citaId=5
        public async Task<IActionResult> Create(int citaId)
        {
            try
            {
                var estadoCuenta = await _pagoService.ObtenerEstadoCuentaCitaAsync(citaId);

                var vm = new PagoCreateViewModel
                {
                    EstadoCuenta = estadoCuenta,
                    Pago = new PagoDtoIn
                    {
                        CitaId = citaId,
                        Monto = estadoCuenta.Pendiente
                    },
                    MetodosPago = ObtenerMetodosPagoList()
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "No se pudo cargar la información del pago: " + ex.Message;
                return RedirectToAction("Details", "Cita", new { id = citaId });
            }
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PagoCreateViewModel vm)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    await _pagoService.RegistrarPagoAsync(vm.Pago);

                    TempData["MensajeExito"] = "Pago registrado correctamente.";

                    return RedirectToAction("Details", "Cita", new { id = vm.Pago.CitaId });
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al procesar el pago.");
                }
            }

            // --- MANEJO DE ERRORES ---
            try
            {
                vm.EstadoCuenta = await _pagoService.ObtenerEstadoCuentaCitaAsync(vm.Pago.CitaId);
                vm.MetodosPago = ObtenerMetodosPagoList();
            }
            catch
            {
                // Si falla incluso recargar el estado, mejor abortamos
                TempData["MensajeError"] = "Error crítico al recuperar datos de la cita.";
                return RedirectToAction("Index", "Cita");
            }

            return View(vm);
        }
    }
}
