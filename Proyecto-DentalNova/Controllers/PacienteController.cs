using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.PacienteViewModel;

namespace Proyecto_DentalNova.Controllers
{

    public class PacienteController : Controller
    {
        private readonly IPacienteService _pacienteService;
        private readonly ICitaService _citaService;

        public PacienteController(IPacienteService pacienteService, ICitaService citaService)
        {
            _pacienteService = pacienteService;
            _citaService = citaService;
        }

        // --- MÉTODO AUXILIAR ---
        private async Task<PacienteVM> BuildPacienteVMAsync(PacienteAdminDtoIn? pacienteIn = null)
        {
            // Obtenemos el ID si estamos editando, para que la API sepa qué usuario NO excluir
            int? pacienteIdEdicion = (pacienteIn != null && pacienteIn.Id > 0) ? pacienteIn.Id : null;

            // Llamamos a la API 
            var usuariosDisponibles = await _pacienteService.ObtenerUsuariosDisponiblesAsync(pacienteIdEdicion);

            return new PacienteVM
            {
                // Si es null, inicializamos uno nuevo
                Paciente = pacienteIn ?? new PacienteAdminDtoIn(),
                UsuariosDisponibles = usuariosDisponibles
            };
        }

        // --- GET: Index ---
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] PacienteFilterViewModel filtro)
        {
            // ViewModel de filtro -> DTO de Filtro para la API
            var filtroDto = new PacienteFilterDto
            {
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                Id = filtro.Id,
                NombreLike = filtro.NombreLike,
                ApellidosLike = filtro.ApellidosLike,
                CorreoLike = filtro.CorreoLike,
                TelefonoLike = filtro.TelefonoLike,
                EdadMin = filtro.EdadMin,
                EdadMax = filtro.EdadMax,
                FechaDesde = filtro.FechaDesde,
                FechaHasta = filtro.FechaHasta,
                ConAlergias = filtro.ConAlergias,
                ConEnfermedadesCronicas = filtro.ConEnfermedadesCronicas,
                ConMedicamentosActuales = filtro.ConMedicamentosActuales,
                ConAntecedentesFamiliares = filtro.ConAntecedentesFamiliares
            };

            // Llamada a la API
            var apiResult = await _pacienteService.ObtenerPacientesAsync(filtroDto);

            // Construir la lista paginada
            var pagedResults = PaginatedList<PacienteAdminDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                apiResult.PageIndex,
                filtro.PageSize);

            var vm = new PacienteIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedResults
            };

            return View(vm);
        }

        // --- GET: Details ---
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dtoOut = await _pacienteService.ObtenerPacientePorIdAsync(id.Value);
                return View(dtoOut);
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        // --- GET: Create ---
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildPacienteVMAsync();
            return View(vm);
        }

        // --- POST: Create ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(PacienteVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Enviamos el DTO de entrada directamente a la API
                    await _pacienteService.CrearPacienteAsync(vm.Paciente);

                    TempData["MensajeExito"] = "Paciente creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var reloadedVm = await BuildPacienteVMAsync(vm.Paciente);
            TempData["MensajeError"] = "Error al crear el paciente. Por favor, revise los datos e intente nuevamente.";
            return View(reloadedVm);
        }

        // --- GET: Edit ---
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                // Obtenemos el DTO de Salida
                var dtoOut = await _pacienteService.ObtenerPacientePorIdAsync(id.Value);

                var dtoIn = new PacienteAdminDtoIn
                {
                    Id = dtoOut.Id,
                    UsuarioId = dtoOut.UsuarioId,
                    Edad = dtoOut.Edad,
                    ConAlergias = dtoOut.ConAlergias,
                    Alergias = dtoOut.Alergias,
                    ConEnfermedadesCronicas = dtoOut.ConEnfermedadesCronicas,
                    EnfermedadesCronicas = dtoOut.EnfermedadesCronicas,
                    ConMedicamentosActuales = dtoOut.ConMedicamentosActuales,
                    MedicamentosActuales = dtoOut.MedicamentosActuales,
                    ConAntecedentesFamiliares = dtoOut.ConAntecedentesFamiliares,
                    AntecedentesFamiliares = dtoOut.AntecedentesFamiliares,
                    Observaciones = dtoOut.Observaciones
                };

                var vm = await BuildPacienteVMAsync(dtoIn);
                return View(vm);
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        // --- POST: Edit ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, PacienteVM vm)
        {
            if (id != vm.Paciente.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _pacienteService.ActualizarPacienteAsync(id, vm.Paciente);

                    TempData["MensajeExito"] = "Paciente actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var reloadedVm = await BuildPacienteVMAsync(vm.Paciente);
            TempData["MensajeError"] = "Error al actualizar el paciente. Por favor, revise los datos e intente nuevamente.";
            return View(reloadedVm);
        }

        // --- GET: Delete ---
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dtoOut = await _pacienteService.ObtenerPacientePorIdAsync(id.Value);
                return View(dtoOut); // La vista Delete espera PacienteDto
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        // --- POST: Delete ---
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _pacienteService.EliminarPacienteAsync(id);
                TempData["MensajeExito"] = "Paciente eliminado correctamente.";
            }
            catch (HttpRequestException ex)
            {
                TempData["MensajeError"] = "Error al eliminar: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }


        //[Authorize(Roles = "Odontologo")]
        //public async Task<IActionResult> DetailsOdontologo(int id)
        //{
        //    var paciente = await _pacienteService.ObtenerPacientePorIdAsync(id);
        //    if (paciente == null) return NotFound();

        //    // Obtener Historial de Citas (Usamos el filtro por PacienteId)
        //    var filtroCitas = new CitaFilterDto
        //    {
        //        PacienteId = id,
        //        PageSize = 50,
        //    };

        //    var citasResult = await _citaService.ObtenerListaPaginadaAsync(filtroCitas);

        //    // Ordenamos descendente
        //    var historial = citasResult.Items.OrderByDescending(c => c.FechaHora).ToList();

        //    var vm = new PacienteDetalleViewModel
        //    {
        //        Paciente = paciente,
        //        HistorialCitas = historial,
        //        TotalCitas = citasResult.TotalCount,
        //        UltimaVisita = historial.FirstOrDefault(c => c.EstatusCita == Enumerables.EstatusCita.Completada)?.FechaHora
        //    };

        //    return View(vm);
        //}
    }
}