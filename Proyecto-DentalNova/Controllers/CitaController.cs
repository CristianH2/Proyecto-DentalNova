using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;
using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.CitaViewModel;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace Proyecto_DentalNova.Controllers
{
    public class CitaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Método auxiliar para el filtro ---
        private void HydrateFilter(CitaFilterViewModel filtro)
        {
            filtro.EstatusDisponibles = Enum.GetValues<EstatusCita>()
                .Select(e => new SelectListItem(e.ToString(), e.ToString()))
                .ToList();
        }

        // --- Método para construir el View Model ---
        private async Task<CitaVM> BuildCitaVMAsync(Cita? cita = null)
        {
            var vm = new CitaVM
            {
                Cita = cita ?? new Cita { FechaHora = DateTime.Now },

                // Carga solo los pacientes que tienen un usuario activo asociado
                PacientesDisponibles = await _context.Pacientes
                    .Include(p => p.Usuario)
                    .Where(p => p.Usuario.Activo)
                    .OrderBy(p => p.Usuario.Apellidos)
                    .Select(p => new SelectListItem(
                        $"{p.Usuario.Apellidos}, {p.Usuario.Nombre}",
                        p.Id.ToString()))
                    .ToListAsync(),

                // Carga solo los odontólogos que tienen un usuario activo asociado
                OdontologosDisponibles = await _context.Odontologos
                    .Include(o => o.Usuario)
                    .Where(o => o.Usuario.Activo)
                    .OrderBy(o => o.Usuario.Apellidos)
                    .Select(o => new SelectListItem(
                        $"{o.Usuario.Apellidos}, {o.Usuario.Nombre}",
                        o.Id.ToString()))
                    .ToListAsync(),

                // Carga los estatus desde el Enum
                EstatusDisponibles = Enum.GetValues<EstatusCita>()
                    .Select(e => new SelectListItem(e.ToString(), e.ToString()))
                    .ToList(),

                // Carga las duraciones para los Radio Buttons
                DuracionesDisponibles = Enum.GetValues<DuracionMinutos>()
                    .Select(d => new SelectListItem(
                        $"{(int)d} minutos", // Muestra "30 minutos", "60 minutos", etc.
                        d.ToString()))
                    .ToList()
            };
            return vm;
        }

        // GET: Cita
        // --- ACCIÓN INDEX ACTUALIZADA ---
        [HttpGet]
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] CitaFilterViewModel filtro)
        {
            HydrateFilter(filtro);

            IQueryable<Cita> query = _context.Citas
                .Include(c => c.Paciente.Usuario)
                .Include(c => c.Odontologo.Usuario)
                .AsNoTracking();

            if (filtro.Id.HasValue)
                query = query.Where(c => c.Id == filtro.Id.Value);

            // Filtra por nombre O apellido del paciente
            if (!string.IsNullOrWhiteSpace(filtro.PacienteNombreLike))
                query = query.Where(c => c.Paciente.Usuario.Nombre.Contains(filtro.PacienteNombreLike) ||
                                         c.Paciente.Usuario.Apellidos.Contains(filtro.PacienteNombreLike));

            // Filtra por nombre O apellido del odontólogo
            if (!string.IsNullOrWhiteSpace(filtro.OdontologoNombreLike))
                query = query.Where(c => c.Odontologo.Usuario.Nombre.Contains(filtro.OdontologoNombreLike) ||
                                         c.Odontologo.Usuario.Apellidos.Contains(filtro.OdontologoNombreLike));

            if (filtro.FechaDesde.HasValue)
                query = query.Where(c => c.FechaHora.Date >= filtro.FechaDesde.Value);
            if (filtro.FechaHasta.HasValue)
                query = query.Where(c => c.FechaHora.Date <= filtro.FechaHasta.Value);
            if (filtro.Estatus.HasValue)
                query = query.Where(c => c.EstatusCita == filtro.Estatus.Value);

            query = query.OrderByDescending(c => c.FechaHora);
            var pagedResults = await PaginatedList<Cita>.CreateAsync(query, filtro.Page, filtro.PageSize);
            var vm = new CitaIndexViewModel { Filtro = filtro, Resultados = pagedResults };

            return View(vm);
        }

        // --- Acciones para el autocompletado (JSON) ---

        [HttpGet]
        public async Task<IActionResult> BuscarPacientes(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            // Búsqueda más robusta: comprueba si el término está en el nombre O en los apellidos.
            var pacientes = await _context.Pacientes
                .Include(p => p.Usuario)
                .Where(p => p.Usuario.Nombre.Contains(term) || p.Usuario.Apellidos.Contains(term))
                .Take(10)
                .Select(p => new {
                    id = p.Id,
                    label = $"{p.Usuario.Nombre} {p.Usuario.Apellidos}"
                })
                .ToListAsync();

            return Json(pacientes);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarOdontologos(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            // Búsqueda más robusta: comprueba si el término está en el nombre O en los apellidos.
            var odontologos = await _context.Odontologos
                .Include(o => o.Usuario)
                .Where(o => o.Usuario.Nombre.Contains(term) || o.Usuario.Apellidos.Contains(term))
                .Take(10)
                .Select(o => new {
                    id = o.Id,
                    label = $"{o.Usuario.Nombre} {o.Usuario.Apellidos}"
                })
                .ToListAsync();

            return Json(odontologos);
        }

        // GET: Cita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Paciente.Usuario)
                .Include(c => c.Odontologo.Usuario)
                // Carga ansiosa de los detalles (CitaTratamiento) Y sus tratamientos asociados
                .Include(c => c.CitasTratamientos)
                    .ThenInclude(ct => ct.Tratamiento)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cita == null) return NotFound();

            return View(cita);
        }

        // GET: Cita/Create
        public async Task<IActionResult> Create()
        {
            var vm = await BuildCitaVMAsync();
            return View(vm);
        }

        // POST: Cita/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitaVM vm)
        {
            if (ModelState.IsValid)
            {
                vm.Cita.FechaCreacion = DateTime.Now;
                _context.Add(vm.Cita);
                await _context.SaveChangesAsync();
                // Redirige a la nueva vista de detalles para añadir tratamientos
                return RedirectToAction(nameof(Details), new { id = vm.Cita.Id });
            }

            var reloadedVm = await BuildCitaVMAsync(vm.Cita);
            return View(reloadedVm);
        }

        // GET: Cita/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound();

            var vm = await BuildCitaVMAsync(cita);
            return View(vm);
        }

        // POST: Cita/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CitaVM vm)
        {
            if (id != vm.Cita.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                vm.Cita.FechaActualizacion = DateTime.Now;
                _context.Update(vm.Cita);
                await _context.SaveChangesAsync();
                // Redirige de vuelta a la vista de detalles
                return RedirectToAction(nameof(Details), new { id = vm.Cita.Id });
            }

            var reloadedVm = await BuildCitaVMAsync(vm.Cita);
            return View(reloadedVm);
        }

        // GET: Cita/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cita = await _context.Citas
                .Include(c => c.Paciente.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cita == null) return NotFound();
            return View(cita);
        }

        // POST: Cita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                // Si la cita tiene tratamientos. Se eliminará en cascada
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.Id == id);
        }
    }
}
