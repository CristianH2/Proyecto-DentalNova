using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.OdontologoViewModel;


namespace Proyecto_DentalNova.Controllers
{
    // [Authorize(Roles = "Administrador")]
    public class OdontologoController : Controller
    {
        private readonly IOdontologoService _odontologoService;

        public OdontologoController(IOdontologoService odontologoService)
        {
            _odontologoService = odontologoService;
        }

        // Método auxiliar para el filtro
        private async Task HydrateFilterAsync(OdontologoFilterViewModel filtro)
        {
            var especialidades = await _odontologoService.ObtenerEspecialidadesAsync();
            filtro.EspecialidadesDisponibles = especialidades
                .OrderBy(e => e.Nombre)
                .Select(e => new SelectListItem(e.Nombre, e.Id.ToString()));
        }

        private async Task<OdontologoVM> BuildOdontologoVMAsync(OdontologoDtoIn? dtoIn = null)
        {
            // IDs de especialidades seleccionadas (si estamos editando)
            var idsSeleccionados = dtoIn?.EspecialidadesIds?.ToArray() ?? Array.Empty<int>();

            // Obtener datos auxiliares de la API
            var usuariosDisponibles = await _odontologoService.ObtenerUsuariosDisponiblesAsync(dtoIn?.Id > 0 ? dtoIn.Id : null);
            var todasEspecialidades = await _odontologoService.ObtenerEspecialidadesAsync();

            var vm = new OdontologoVM
            {
                Odontologo = dtoIn ?? new OdontologoDtoIn { FechaIngreso = DateTime.Today },

                UsuariosDisponibles = usuariosDisponibles.Select(u => new SelectListItem
                {
                    Text = u.TextoMostrar,
                    Value = u.Id.ToString()
                }).ToList(),

                TodasLasEspecialidades = todasEspecialidades.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre,
                    Selected = idsSeleccionados.Contains(e.Id)
                }).ToList(),

                EspecialidadesSeleccionadasIds = idsSeleccionados
            };

            return vm;
        }

        // GET: Odontologo
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] OdontologoFilterViewModel filtro)
        {
            await HydrateFilterAsync(filtro);

            var filtroDto = new OdontologoFilterDto
            {
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                Id = filtro.Id,
                NombreLike = filtro.NombreLike,
                ApellidosLike = filtro.ApellidosLike,
                CorreoLike = filtro.CorreoLike,
                CedulaLike = filtro.CedulaLike,
                EspecialidadId = filtro.EspecialidadId,
                FechaIngresoDesde = filtro.FechaIngresoDesde,
                FechaIngresoHasta = filtro.FechaIngresoHasta
            };

            // Llamada API
            var apiResult = await _odontologoService.ObtenerOdontologosAsync(filtroDto);

            var pagedResults = PaginatedList<OdontologoDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                apiResult.PageIndex,
                filtro.PageSize);

            var vm = new OdontologoIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedResults
            };

            return View(vm);
        }

        // GET: Odontologo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dto = await _odontologoService.ObtenerOdontologoPorIdAsync(id.Value);
                return View(dto);
            }
            catch (HttpRequestException) { return NotFound(); }
        }

        // GET: Odontologo/Create
        public async Task<IActionResult> Create()
        {
            var vm = await BuildOdontologoVMAsync();
            return View(vm);
        }

        // POST: Odontologo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OdontologoVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dtoIn = vm.Odontologo;
                    // Asignamos las especialidades seleccionadas en la lista de IDs
                    dtoIn.EspecialidadesIds = vm.EspecialidadesSeleccionadasIds?.ToList() ?? new List<int>();

                    await _odontologoService.CrearOdontologoAsync(dtoIn);
                    TempData["MensajeExito"] = "Odontólogo creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var reloadedVm = await BuildOdontologoVMAsync(vm.Odontologo);
            TempData["Error"] = "Error al crear el odontólogo. Por favor, revise los datos e intente nuevamente.";
            return View(reloadedVm);
        }

        // GET: Odontologo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dtoOut = await _odontologoService.ObtenerOdontologoPorIdAsync(id.Value);

                // Convertir DTO Salida -> DTO Entrada
                var dtoIn = new OdontologoDtoIn
                {
                    Id = dtoOut.Id,
                    UsuarioId = dtoOut.UsuarioId,
                    CedulaProfesional = dtoOut.CedulaProfesional,
                    AnioGraduacion = dtoOut.AnioGraduacion,
                    Institucion = dtoOut.Institucion,
                    FechaIngreso = dtoOut.FechaIngreso,
                    EspecialidadesIds = dtoOut.EspecialidadesIds
                };

                var vm = await BuildOdontologoVMAsync(dtoIn);
                return View(vm);
            }
            catch (HttpRequestException) { return NotFound(); }
        }

        // POST: Odontologo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OdontologoVM vm)
        {
            if (id != vm.Odontologo.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var dtoIn = vm.Odontologo;
                    dtoIn.EspecialidadesIds = vm.EspecialidadesSeleccionadasIds?.ToList() ?? new List<int>();

                    await _odontologoService.ActualizarOdontologoAsync(id, dtoIn);
                    TempData["MensajeExito"] = "Odontólogo actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var reloadedVm = await BuildOdontologoVMAsync(vm.Odontologo);
            TempData["Error"] = "Error al actualizar el odontólogo. Por favor, revise los datos e intente nuevamente.";
            return View(reloadedVm);
        }


        // GET: Odontologo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var dtoOut = await _odontologoService.ObtenerOdontologoPorIdAsync(id.Value);
                return View(dtoOut); // La vista Delete espera OdontologoDto
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        // POST: Odontologo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _odontologoService.EliminarOdontologoAsync(id);
                TempData["MensajeExito"] = "Odontólogo eliminado correctamente.";
            }
            catch (HttpRequestException ex)
            {
                TempData["MensajeError"] = "Error al eliminar: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
