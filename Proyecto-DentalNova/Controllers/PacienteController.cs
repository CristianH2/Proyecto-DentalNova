using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;
using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.PacienteViewModel;

namespace Proyecto_DentalNova.Controllers
{
    public class PacienteController : Controller
    {
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
        }
    }
}
