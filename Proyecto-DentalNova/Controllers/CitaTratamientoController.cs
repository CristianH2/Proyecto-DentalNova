using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.CitaTratamientoViewModel;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace Proyecto_DentalNova.Controllers
{
    public class CitaTratamientoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitaTratamientoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODO AUXILIAR PARA CONSTRUIR EL VIEWMODEL ---
        private async Task<CitaTratamientoVM> BuildVMAsync()
        {
            var vm = new CitaTratamientoVM
            {
                // Carga solo los tratamientos ACTIVOS
                TratamientosDisponibles = await _context.Tratamientos
                    .Where(t => t.Activo)
                    .OrderBy(t => t.Nombre)
                    .Select(t => new TratamientoDisponible(t.Id, t.Nombre, t.Costo))
                    .ToListAsync(),

                // Carga los estatus desde el Enum
                EstatusDisponibles = Enum.GetValues<EstatusTratamiento>()
                    .Select(e => new SelectListItem(e.ToString(), e.ToString()))
                    .ToList()
            };
            return vm;
        }


        // GET: CitaTratamiento/Create?citaId=5
        public async Task<IActionResult> Create(int citaId)
        {
            if (citaId == 0) return BadRequest("Se requiere el ID de la cita.");

            var vm = await BuildVMAsync();

            // Asignamos el ID de la Cita (Maestro) al nuevo Detalle
            vm.CitaTratamiento.CitaId = citaId;

            return View(vm);
        }

        // POST: CitaTratamiento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitaTratamientoVM vm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vm.CitaTratamiento);
                await _context.SaveChangesAsync();

                // Redirige de vuelta a los detalles de la Cita Maestra
                return RedirectToAction("Details", "Cita", new { id = vm.CitaTratamiento.CitaId });
            }

            // Si hay un error, recargamos las listas y volvemos a la vista
            var reloadedVm = await BuildVMAsync();
            reloadedVm.CitaTratamiento = vm.CitaTratamiento; // Mantenemos los datos que el usuario ingresó
            return View(reloadedVm);
        }

        // GET: CitaTratamiento/Edit/10
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var citaTratamiento = await _context.CitasTratamientos.FindAsync(id);
            if (citaTratamiento == null) return NotFound();

            var vm = await BuildVMAsync();
            vm.CitaTratamiento = citaTratamiento;

            return View(vm);
        }

        // POST: CitaTratamiento/Edit/10
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CitaTratamientoVM vm)
        {
            if (id != vm.CitaTratamiento.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(vm.CitaTratamiento);
                await _context.SaveChangesAsync();

                // Redirige de vuelta a los detalles de la Cita Maestra
                return RedirectToAction("Details", "Cita", new { id = vm.CitaTratamiento.CitaId });
            }

            var reloadedVm = await BuildVMAsync();
            reloadedVm.CitaTratamiento = vm.CitaTratamiento;
            return View(reloadedVm);
        }

        // GET: CitaTratamiento/Delete/10
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var citaTratamiento = await _context.CitasTratamientos
                .Include(ct => ct.Tratamiento) // Incluir el nombre del tratamiento
                .Include(ct => ct.Cita)      // Incluir datos de la cita
                .FirstOrDefaultAsync(m => m.Id == id);

            if (citaTratamiento == null) return NotFound();

            return View(citaTratamiento);
        }

        // POST: CitaTratamiento/Delete/10
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var citaTratamiento = await _context.CitasTratamientos.FindAsync(id);
            if (citaTratamiento != null)
            {
                _context.CitasTratamientos.Remove(citaTratamiento);
                await _context.SaveChangesAsync();

                // Redirige de vuelta a los detalles de la Cita Maestra
                return RedirectToAction("Details", "Cita", new { id = citaTratamiento.CitaId });
            }
            // Si algo falla, redirige al inicio
            return RedirectToAction("Index", "Home");
        }
    }
}