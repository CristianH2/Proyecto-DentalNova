using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Data;
using Proyecto_DentalNova.Models;
using Proyecto_DentalNova.Models.OdontologoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_DentalNova.Controllers
{
    public class OdontologoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OdontologoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método auxiliar para construir el ViewModel
        private async Task<OdontologoVM> BuildOdontologoVMAsync(Odontologo? odontologo = null)
        {
            // IDs de usuarios que ya son pacientes.
            var idsUsuariosPacientes = await _context.Pacientes
                                               .Select(p => p.UsuarioId)
                                               .ToListAsync();

            // IDs de usuarios que ya son odontólogos.
            var idsUsuariosOdontologos = await _context.Odontologos
                                                 .Select(o => o.UsuarioId)
                                                 .ToListAsync();

            // Combinar ambas listas para tener todos los IDs "ocupados".
            var idsUsuariosOcupados = idsUsuariosPacientes.Union(idsUsuariosOdontologos).ToList();

            // Consulta para usuarios activos.
            var queryUsuarios = _context.Usuarios.AsNoTracking().Where(u => u.Activo);

            if (odontologo == null) // Para el formulario de CREAR
            {
                // Excluir a todos los usuarios que ya tienen un rol (paciente u odontólogo).
                queryUsuarios = queryUsuarios.Where(u => !idsUsuariosOcupados.Contains(u.Id));
            }
            else // Para el formulario de EDITAR
            {
                // Excluir a los usuarios ocupados, EXCEPTO el que ya está asignado a este odontólogo.
                queryUsuarios = queryUsuarios.Where(u => !idsUsuariosOcupados.Contains(u.Id) || u.Id == odontologo.UsuarioId);
            }

            // Obtener los IDs de las especialidades que el odontólogo ya tiene (para el modo Edición)
            var especialidadesDelOdontologoIds = new HashSet<int>();
            if (odontologo != null)
            {
                // Si estamos editando, cargamos explícitamente las especialidades asociadas
                var odontologoConEspecialidades = await _context.Odontologos
                    .Include(o => o.Especialidades)
                    .FirstOrDefaultAsync(o => o.Id == odontologo.Id);
                if (odontologoConEspecialidades != null)
                {
                    especialidadesDelOdontologoIds = odontologoConEspecialidades.Especialidades.Select(e => e.Id).ToHashSet();
                }
            }

            var vm = new OdontologoVM
            {
                Odontologo = odontologo ?? new Odontologo { FechaIngreso = DateTime.Today },
                UsuariosDisponibles = await queryUsuarios
                    .OrderBy(u => u.Apellidos)
                    .Select(u => new SelectListItem
                    {
                        Text = $"{u.Apellidos}, {u.Nombre} ({u.CorreoElectronico})",
                        Value = u.Id.ToString()
                    }).ToListAsync(),

                    // Mapear todas las especialidades a SelectListItem, marcando las que ya están seleccionadas
                TodasLasEspecialidades = await _context.Especialidades
                    .OrderBy(e => e.Nombre)
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Nombre,
                        Selected = especialidadesDelOdontologoIds.Contains(e.Id)
                    }).ToListAsync()
            };

            return vm;
        }

        // GET: Odontologo
        public async Task<IActionResult> Index()
        {
            var odontologos = await _context.Odontologos
                                        .Include(o => o.Usuario) // Cargar datos del usuario
                                        .Include(o => o.Especialidades) // Cargar las especialidades asociadas
                                        .Where(o => o.Usuario.Activo)
                                        .OrderBy(o => o.Usuario.Apellidos)
                                        .ToListAsync();
            return View(odontologos);
        }

        // GET: Odontologo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var odontologo = await _context.Odontologos
                .Include(o => o.Usuario)
                .Include(o => o.Especialidades)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (odontologo == null)
            {
                return NotFound();
            }

            return View(odontologo);
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
            // Verificación extra para evitar duplicados (race condition)
            if (await _context.Pacientes.AnyAsync(p => p.UsuarioId == vm.Odontologo.UsuarioId) ||
                await _context.Odontologos.AnyAsync(o => o.UsuarioId == vm.Odontologo.UsuarioId))
            {
                ModelState.AddModelError("Odontologo.UsuarioId", "Este usuario ya ha sido asignado a otro rol.");
            }

            if (ModelState.IsValid)
            {
                // Asignar las especialidades seleccionadas desde el arreglo de IDs
                if (vm.EspecialidadesSeleccionadasIds != null)
                {
                    vm.Odontologo.Especialidades = await _context.Especialidades
                        .Where(e => vm.EspecialidadesSeleccionadasIds.Contains(e.Id))
                        .ToListAsync();
                }

                _context.Add(vm.Odontologo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var reloadedVm = await BuildOdontologoVMAsync(vm.Odontologo);
            return View(reloadedVm);
        }

        // GET: Odontologo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var odontologo = await _context.Odontologos.FindAsync(id);
            if (odontologo == null) return NotFound();

            var vm = await BuildOdontologoVMAsync(odontologo);
            return View(vm);
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
                var odontologoParaActualizar = await _context.Odontologos
                    .Include(o => o.Especialidades)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (odontologoParaActualizar == null) return NotFound();

                // Actualizar propiedades del odontólogo
                _context.Entry(odontologoParaActualizar).CurrentValues.SetValues(vm.Odontologo);

                // Actualizar la lista de especialidades
                odontologoParaActualizar.Especialidades.Clear();
                if (vm.EspecialidadesSeleccionadasIds != null)
                {
                    var nuevasEspecialidades = await _context.Especialidades
                        .Where(e => vm.EspecialidadesSeleccionadasIds.Contains(e.Id))
                        .ToListAsync();
                    odontologoParaActualizar.Especialidades = nuevasEspecialidades;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var reloadedVm = await BuildOdontologoVMAsync(vm.Odontologo);
            return View(reloadedVm);
        }

        // GET: Odontologo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var odontologo = await _context.Odontologos
                .Include(o => o.Usuario)
                .Include(o => o.Especialidades)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (odontologo == null)
            {
                return NotFound();
            }

            return View(odontologo);
        }

        // POST: Odontologo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var odontologo = await _context.Odontologos.FindAsync(id);
            if (odontologo != null)
            {
                _context.Odontologos.Remove(odontologo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OdontologoExists(int id)
        {
            return _context.Odontologos.Any(e => e.Id == id);
        }
    }
}
