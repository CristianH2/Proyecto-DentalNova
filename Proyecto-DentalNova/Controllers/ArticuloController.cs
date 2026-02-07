using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.ArticuloViewModel;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace Proyecto_DentalNova.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ArticuloController : Controller
    {
        private readonly IArticuloService _articuloService;

        public ArticuloController(IArticuloService articuloService)
        {
            _articuloService = articuloService;
        }

        // --- Helper ---
        private IEnumerable<SelectListItem> ObtenerCategoriasSelectList()
        {
            // Convierte el Enum Categoria a SelectListItem
            return Enum.GetValues(typeof(Categoria)).Cast<Categoria>()
                .Select(c => new SelectListItem
                {
                    Value = ((int)c).ToString(), // Enviamos el int al servidor
                    Text = c.ToString()          // Mostramos el nombre
                });
        }

        // --- GET: Articulo ---
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] ArticuloFilterDto filtro)
        {
            // Valores por defecto
            if (filtro.Page < 1) filtro.Page = 1;
            if (filtro.PageSize < 1) filtro.PageSize = 10;

            // Llamada a API
            var apiResult = await _articuloService.ObtenerListaPaginadaAsync(filtro);

            // Conversión a PaginatedList
            var listaPaginada = PaginatedList<ArticuloDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                filtro.Page,
                filtro.PageSize
            );

            // Construir ViewModel
            var vm = new ArticuloIndexViewModel
            {
                Filtro = filtro,
                Resultados = listaPaginada,
                Categorias = ObtenerCategoriasSelectList()
            };

            return View(vm);
        }

        // --- GET: Articulo/Create ---
        public IActionResult Create()
        {
            var vm = new ArticuloVM
            {
                Categorias = ObtenerCategoriasSelectList()
            };
            return View(vm);
        }

        // --- POST: Articulo/Create ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticuloVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _articuloService.CrearAsync(vm.Articulo);
                    TempData["MensajeExito"] = "Artículo registrado correctamente en el inventario.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    // Errores de API (ej: Código duplicado)
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear el artículo.");
                }
            }

            // Si falla, recargamos la lista
            vm.Categorias = ObtenerCategoriasSelectList();
            TempData["MensajeError"] = "No se pudo guardar el artículo. Verifique los errores.";
            return View(vm);
        }

        // --- GET: Articulo/Edit/5 ---
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _articuloService.ObtenerParaEditarAsync(id);
                var vm = new ArticuloVM
                {
                    Articulo = dto,
                    Categorias = ObtenerCategoriasSelectList()
                };
                return View(vm);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "El artículo solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

        // --- POST: Articulo/Edit/5 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArticuloVM vm)
        {
            if (id != vm.Articulo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _articuloService.ActualizarAsync(vm.Articulo);
                    TempData["MensajeExito"] = "Información del artículo actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el artículo.");
                }
            }

            vm.Categorias = ObtenerCategoriasSelectList();
            return View(vm);
        }

        // --- POST: Articulo/Delete/5 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _articuloService.EliminarAsync(id);
                TempData["MensajeExito"] = "Artículo dado de baja correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al eliminar: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // --- POST: Articulo/CambiarEstatus/5 ---
        // Este lo usaremos con AJAX para el toggle switch en el Index
        [HttpPost]
        public async Task<IActionResult> CambiarEstatus(int id)
        {
            try
            {
                await _articuloService.CambiarEstatusAsync(id);
                return Json(new { success = true, message = "Estatus actualizado." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
