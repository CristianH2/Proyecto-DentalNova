using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.PacienteViewModel;

namespace Proyecto_DentalNova.Controllers
{
    // [Authorize(Roles = "Administrador, Odontologo")]
    public class PacienteController : Controller
    {
        private readonly IPacienteService _pacienteService;

        public PacienteController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        // --- MÉTODO AUXILIAR ---
        private async Task<PacienteVM> BuildPacienteVMAsync(PacienteAdminDtoIn? pacienteIn = null)
        {
            // Obtenemos el ID si estamos editando, para que la API sepa qué usuario NO excluir
            int? pacienteIdEdicion = (pacienteIn != null && pacienteIn.Id > 0) ? pacienteIn.Id : null;

            // Llamamos a la API para obtener la lista de usuarios (ya filtrada)
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
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] PacienteFilterViewModel filtro)
        {
            // 1. Mapear ViewModel de filtro -> DTO de Filtro para la API
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

            // 2. Llamada a la API
            var apiResult = await _pacienteService.ObtenerPacientesAsync(filtroDto);

            // 3. Construir la lista paginada usando los DTOs de salida
            // (Usamos el método estático .Create que agregamos a PaginatedList)
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                // La API devuelve un DTO de salida con toda la info (Nombre usuario, edad, etc.)
                var dtoOut = await _pacienteService.ObtenerPacientePorIdAsync(id.Value);
                return View(dtoOut); // La vista Details ahora espera PacienteAdminDtoOut
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        // --- GET: Create ---
        public async Task<IActionResult> Create()
        {
            var vm = await BuildPacienteVMAsync();
            return View(vm);
        }

        // --- POST: Create ---
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    // Capturamos errores de negocio (ej. usuario ya asignado)
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            // Si falla, reconstruimos el VM (listas desplegables, etc.)
            var reloadedVm = await BuildPacienteVMAsync(vm.Paciente);
            TempData["MensajeError"] = "Error al crear el paciente. Por favor, revise los datos e intente nuevamente.";
            return View(reloadedVm);
        }

        // --- GET: Edit ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                // 1. Obtenemos el DTO de Salida
                var dtoOut = await _pacienteService.ObtenerPacientePorIdAsync(id.Value);

                // 2. Lo convertimos a DTO de Entrada para el formulario
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
    }






    /*
    private readonly ApplicationDbContext _context;

    public PacienteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Método auxiliar para construir el ViewModel
    private async Task<PacienteVM> BuildPacienteVMAsync(Paciente? paciente = null)
    {
        // Obtener los IDs de los usuarios que ya son pacientes.
        var idsUsuariosOcupados = await _context.Pacientes
                                         .Select(p => p.UsuarioId)
                                         .ToListAsync();

        // Preparar la consulta para los usuarios disponibles.
        var queryUsuarios = _context.Usuarios.AsNoTracking();
        queryUsuarios = queryUsuarios.Where(u => u.Activo);

        if (paciente == null) // Para el formulario de CREAR
        {
            // Excluir todos los usuarios que ya son pacientes.
            queryUsuarios = queryUsuarios.Where(u => !idsUsuariosOcupados.Contains(u.Id));
        }
        else // Para el formulario de EDITAR
        {
            // Excluir a los usuarios que son pacientes, EXCEPTO el que está asignado a este paciente.
            queryUsuarios = queryUsuarios.Where(u => !idsUsuariosOcupados.Contains(u.Id) || u.Id == paciente.UsuarioId);
        }

        var vm = new PacienteVM
        {
            Paciente = paciente ?? new Paciente(),
            UsuariosDisponibles = await queryUsuarios
            .OrderBy(u => u.Apellidos)
            .Select(u => new UsuarioDisponible(
                u.Id,
                $"{u.Apellidos}, {u.Nombre} ({u.CorreoElectronico})",
                u.FechaNacimiento
            )).ToListAsync()
        };

        return vm;
    }

    // GET: Paciente
    public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] PacienteFilterViewModel filtro)
    {
        // La consulta base incluye al Usuario para el filtro
        IQueryable<Paciente> query = _context.Pacientes.Include(p => p.Usuario).AsNoTracking();

        if (filtro.Id.HasValue) // Si se proporciona un ID, los demás filtros se ignoran para una búsqueda directa.
        {
            query = query.Where(p => p.Id == filtro.Id.Value);
        }
        else
        {
            // Aplicar filtros de Usuario
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike))
                query = query.Where(p => p.Usuario.Nombre.Contains(filtro.NombreLike));
            if (!string.IsNullOrWhiteSpace(filtro.ApellidosLike))
                query = query.Where(p => p.Usuario.Apellidos.Contains(filtro.ApellidosLike));
            if (!string.IsNullOrWhiteSpace(filtro.CorreoLike))
                query = query.Where(p => p.Usuario.CorreoElectronico.Contains(filtro.CorreoLike));
            if (!string.IsNullOrWhiteSpace(filtro.TelefonoLike))
                query = query.Where(p => p.Usuario.Telefono.Contains(filtro.TelefonoLike));

            // Aplicar filtros de Paciente
            if (filtro.EdadMin.HasValue)
                query = query.Where(p => p.Edad >= filtro.EdadMin.Value);
            if (filtro.EdadMax.HasValue)
                query = query.Where(p => p.Edad <= filtro.EdadMax.Value);
            if (filtro.FechaDesde.HasValue)
                query = query.Where(p => p.FechaCreacion.Date >= filtro.FechaDesde.Value);
            if (filtro.FechaHasta.HasValue)
                query = query.Where(p => p.FechaCreacion.Date <= filtro.FechaHasta.Value);

            // Aplicar filtros de Historial Clínico
            if (filtro.ConAlergias)
                query = query.Where(p => p.ConAlergias);
            if (filtro.ConEnfermedadesCronicas)
                query = query.Where(p => p.ConEnfermedadesCronicas);
            if (filtro.ConMedicamentosActuales)
                query = query.Where(p => p.ConMedicamentosActuales);
            if (filtro.ConAntecedentesFamiliares)
                query = query.Where(p => p.ConAntecedentesFamiliares);
        }
        query = query.OrderBy(p => p.Usuario.Apellidos);

        var pagedResults = await PaginatedList<Paciente>.CreateAsync(query, filtro.Page, filtro.PageSize);

        var vm = new PacienteIndexViewModel
        {
            Filtro = filtro,
            Resultados = pagedResults
        };

        return View(vm);
    }

    // GET: Paciente/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var paciente = await _context.Pacientes
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (paciente == null)
        {
            return NotFound();
        }

        return View(paciente);
    }

    // GET: Paciente/Create
    public async Task<IActionResult> Create()
    {
        var vm = await BuildPacienteVMAsync();
        return View(vm);
    }

    // POST: Paciente/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PacienteVM vm)
    {
        // Verificación extra para evitar que se asigne un usuario que ya es paciente (race condition)
        if (await _context.Pacientes.AnyAsync(p => p.UsuarioId == vm.Paciente.UsuarioId))
        {
            ModelState.AddModelError("Paciente.UsuarioId", "Este usuario ya ha sido asignado a otro paciente.");
        }

        if (ModelState.IsValid)
        {
            vm.Paciente.FechaCreacion = DateTime.Now;
            _context.Add(vm.Paciente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Si hay un error, recargar el VM y volver a la vista
        var reloadedVm = await BuildPacienteVMAsync(vm.Paciente);
        return View(reloadedVm);
    }

    // GET: Paciente/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return NotFound();

        var vm = await BuildPacienteVMAsync(paciente);
        return View(vm);
    }

    // POST: Paciente/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PacienteVM vm)
    {
        if (id != vm.Paciente.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                vm.Paciente.FechaActualizacion = DateTime.Now;
                _context.Update(vm.Paciente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pacientes.Any(p => p.Id == vm.Paciente.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        var reloadedVm = await BuildPacienteVMAsync(vm.Paciente);
        return View(reloadedVm);
    }

    // GET: Paciente/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var paciente = await _context.Pacientes
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (paciente == null)
        {
            return NotFound();
        }

        return View(paciente);
    }

    // POST: Paciente/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente != null)
        {
            _context.Pacientes.Remove(paciente);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PacienteExists(int id)
    {
        return _context.Pacientes.Any(e => e.Id == id);
    }*/
}

