using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;
using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.TratamientoViewModel;

namespace Proyecto_DentalNova.Controllers
{
    public class TratamientoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TratamientoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tratamiento
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] TratamientoFilterViewModel filtro)
        {
            IQueryable<Tratamiento> query = _context.Tratamientos.AsNoTracking();

            // Aplicar filtros
            if (filtro.Id.HasValue)
                query = query.Where(t => t.Id == filtro.Id.Value);
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike))
                query = query.Where(t => t.Nombre.Contains(filtro.NombreLike));
            if (filtro.CostoMin.HasValue)
                query = query.Where(t => t.Costo >= filtro.CostoMin.Value);
            if (filtro.CostoMax.HasValue)
                query = query.Where(t => t.Costo <= filtro.CostoMax.Value);
            if (filtro.DuracionMin.HasValue)
                query = query.Where(t => t.DuracionDias >= filtro.DuracionMin.Value);
            if (filtro.DuracionMax.HasValue)
                query = query.Where(t => t.DuracionDias <= filtro.DuracionMax.Value);
            if (filtro.Activo.HasValue)
                query = query.Where(t => t.Activo == filtro.Activo.Value);

            query = query.OrderBy(t => t.Nombre);

            var pagedResults = await PaginatedList<Tratamiento>.CreateAsync(query, filtro.Page, filtro.PageSize);

            var vm = new TratamientoIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedResults
            };

            return View(vm);
        }

        // GET: Tratamiento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tratamiento = await _context.Tratamientos.FirstOrDefaultAsync(m => m.Id == id);
            if (tratamiento == null) return NotFound();
            return View(tratamiento);
        }

        // GET: Tratamiento/Create
        public IActionResult Create()
        {
            return View(new Tratamiento());
        }

        // POST: Tratamiento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Costo,DuracionDias,Activo")] Tratamiento tratamiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tratamiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tratamiento);
        }

        // GET: Tratamiento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento == null) return NotFound();
            return View(tratamiento);
        }

        // POST: Tratamiento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Costo,DuracionDias,Activo")] Tratamiento tratamiento)
        {
            if (id != tratamiento.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tratamiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tratamientos.Any(e => e.Id == tratamiento.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tratamiento);
        }

        // GET: Tratamiento/ToggleActivo/5
        public async Task<IActionResult> ToggleActivo(int? id)
        {
            if (id == null) return NotFound();
            var tratamiento = await _context.Tratamientos.FirstOrDefaultAsync(m => m.Id == id);
            if (tratamiento == null) return NotFound();
            return View(tratamiento);
        }

        // POST: Tratamiento/ToggleActivo/5
        [HttpPost, ActionName("ToggleActivo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivoConfirmed(int id)
        {
            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento != null)
            {
                tratamiento.Activo = !tratamiento.Activo; // Invert the status
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Tratamiento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tratamiento == null)
            {
                return NotFound();
            }

            return View(tratamiento);
        }

        // POST: Tratamiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento != null)
            {
                _context.Tratamientos.Remove(tratamiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TratamientoExists(int id)
        {
            return _context.Tratamientos.Any(e => e.Id == id);
        }
    }
}
