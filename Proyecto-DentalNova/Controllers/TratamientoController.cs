using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Proyecto_DentalNova.Models.TratamientoViewModel;

namespace Proyecto_DentalNova.Controllers
{
    //[Authorize(Roles = "Administrador")] // Seguridad MVC
    public class TratamientoController : Controller
    {
        private readonly ITratamientoService _tratamientoService;

        public TratamientoController(ITratamientoService tratamientoService)
        {
            _tratamientoService = tratamientoService;
        }

        // --- GET: Index ---
        [HttpGet]
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] TratamientoFilterViewModel filtro)
        {
            // 1. Mapear VM Filtro -> DTO Filtro
            var filtroDto = new TratamientoFilterDto
            {
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                Id = filtro.Id,
                NombreLike = filtro.NombreLike,
                CostoMin = filtro.CostoMin,
                CostoMax = filtro.CostoMax,
                Activo = filtro.Activo
            };

            // 2. Llamar API
            var apiResult = await _tratamientoService.ObtenerTratamientosAdminAsync(filtroDto);

            // 3. Crear lista paginada
            var pagedResults = PaginatedList<TratamientoDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                apiResult.PageIndex,
                filtro.PageSize);

            var vm = new TratamientoIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedResults
            };

            return View(vm);
        }

        // --- GET: Details ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dto = await _tratamientoService.ObtenerTratamientoPorIdAsync(id.Value);
                // Nota: Tu vista Details.cshtml debe esperar TratamientoAdminDtoOut
                return View(dto);
            }
            catch (HttpRequestException) { return NotFound(); }
        }

        // --- GET: Create ---
        public IActionResult Create()
        {
            var vm = new TratamientoVM(); // Inicializa con valores por defecto
            return View(vm);
        }

        // --- POST: Create ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TratamientoVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _tratamientoService.CrearTratamientoAsync(vm.Tratamiento);
                    TempData["MensajeExito"] = "Tratamiento creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            TempData["MensajeError"] = "No se pudo crear el tratamiento. Por favor, revise los errores.";
            return View(vm);
        }

        // --- GET: Edit ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dtoOut = await _tratamientoService.ObtenerTratamientoPorIdAsync(id.Value);

                // Mapear Salida -> Entrada para el formulario
                var dtoIn = new TratamientoDtoIn
                {
                    Id = dtoOut.Id,
                    Nombre = dtoOut.Nombre,
                    Descripcion = dtoOut.Descripcion,
                    Costo = dtoOut.Costo,
                    DuracionDias = dtoOut.DuracionDias,
                    Activo = dtoOut.Activo
                };

                var vm = new TratamientoVM { Tratamiento = dtoIn };
                return View(vm);
            }
            catch (HttpRequestException) { return NotFound(); }
        }

        // --- POST: Edit ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TratamientoVM vm)
        {
            if (id != vm.Tratamiento.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _tratamientoService.ActualizarTratamientoAsync(id, vm.Tratamiento);
                    TempData["MensajeExito"] = "Tratamiento actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            TempData["MensajeError"] = "No se pudo actualizar el tratamiento. Por favor, revise los errores.";
            return View(vm);
        }

        // --- GET: Delete ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dto = await _tratamientoService.ObtenerTratamientoPorIdAsync(id.Value);
                // Nota: Tu vista Delete.cshtml debe esperar TratamientoAdminDtoOut
                return View(dto);
            }
            catch (HttpRequestException) { return NotFound(); }
        }

        // --- POST: Delete ---
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _tratamientoService.EliminarTratamientoAsync(id);
                TempData["MensajeExito"] = "Tratamiento eliminado correctamente.";
            }
            catch (HttpRequestException ex)
            {
                TempData["MensajeError"] = "Error al eliminar: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Tratamiento/ToggleActivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            try
            {
                // 1. Obtener el tratamiento actual
                var currentDto = await _tratamientoService.ObtenerTratamientoPorIdAsync(id);

                // 2. Validación de seguridad (por si el ID no existe)
                if (currentDto == null)
                {
                    TempData["MensajeError"] = "El tratamiento solicitado no existe.";
                    return RedirectToAction(nameof(Index));
                }

                // 3. Crear el DTO con el estado INVERTIDO
                var updateDto = new TratamientoDtoIn
                {
                    Id = currentDto.Id,
                    Nombre = currentDto.Nombre,
                    Descripcion = currentDto.Descripcion,
                    Costo = currentDto.Costo,
                    DuracionDias = currentDto.DuracionDias,
                    Activo = !currentDto.Activo // <--- Aquí inviertes el valor
                };

                // 4. Llamar a la API
                await _tratamientoService.ActualizarTratamientoAsync(id, updateDto);

                // 5. CAMBIO CLAVE: Usar las llaves correctas para SweetAlert
                // Antes: TempData["Success"]
                TempData["MensajeExito"] = $"El tratamiento se ha {(updateDto.Activo ? "activado" : "desactivado")} correctamente.";
            }
            catch (Exception ex) // Captura Exception general para atrapar cualquier error, no solo HTTP
            {
                // Antes: TempData["Error"]
                TempData["MensajeError"] = "No se pudo cambiar el estado: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
