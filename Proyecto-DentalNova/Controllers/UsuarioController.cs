using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;
using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Models.UsuarioViewModel;

namespace Proyecto_DentalNova.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método auxiliar para los filtros
        private void HydrateFilter(UsuarioFilterViewModel filtro)
        {
            filtro.Generos = new List<SelectListItem>
        {
            new SelectListItem("Masculino", "M"),
            new SelectListItem("Femenino", "F"),
            new SelectListItem("Otro", "O")
        };

            filtro.Estados = new List<SelectListItem>
        {
            new SelectListItem("Activo", "true"),
            new SelectListItem("Inactivo", "false")
        };
        }

        // Método auxiliar para construir el ViewModel
        private UsuarioVM BuildUsuarioVM(Usuario? usuario = null)
        {
            var vm = new UsuarioVM
            {
                // Si 'usuario' es nulo, crea una nueva instancia con valores por defecto
                Usuario = usuario ?? new Usuario { Activo = true },

                // Carga la lista de géneros para el DropDownList
                Generos = new List<SelectListItem>
                {
                    new SelectListItem("Masculino", "M"),
                    new SelectListItem("Femenino", "F"),
                    new SelectListItem("Otro", "O")
                }
            };

            return vm;
        }

        // Método para obtener la fecha de nacimiento en formato JSON
        [HttpGet]
        public async Task<IActionResult> GetUsuarioFechaNacimiento(int id)
        {
            // Buscamos al usuario por su ID
            var usuario = await _context.Usuarios.AsNoTracking()
                                  .Select(u => new { u.Id, u.FechaNacimiento })
                                  .FirstOrDefaultAsync(u => u.Id == id);

            // Si no tiene fecha de nacimiento, devolvemos null
            if (usuario == null || !usuario.FechaNacimiento.HasValue)
            {
                return Json(new { fechaNacimiento = (string)null });
            }

            // Devolvemos la fecha
            return Json(new { fechaNacimiento = usuario.FechaNacimiento.Value.ToString("yyyy-MM-dd") });
        }

        // GET: Usuario
        [HttpGet]
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] UsuarioFilterViewModel filtro)
        {
            HydrateFilter(filtro);

            IQueryable<Usuario> query = _context.Usuarios.AsNoTracking();

            // Aplicar filtros
            if (filtro.Id.HasValue)
                query = query.Where(u => u.Id == filtro.Id.Value);
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike))
                query = query.Where(u => u.Nombre.Contains(filtro.NombreLike));
            if (!string.IsNullOrWhiteSpace(filtro.ApellidosLike))
                query = query.Where(u => u.Apellidos.Contains(filtro.ApellidosLike));
            if (!string.IsNullOrWhiteSpace(filtro.CorreoLike))
                query = query.Where(u => u.CorreoElectronico.Contains(filtro.CorreoLike));
            if (!string.IsNullOrWhiteSpace(filtro.TelefonoLike))
                query = query.Where(u => u.Telefono.Contains(filtro.TelefonoLike));
            if (filtro.Genero.HasValue)
                query = query.Where(u => u.Genero == filtro.Genero.Value);
            if (filtro.Activo.HasValue)
                query = query.Where(u => u.Activo == filtro.Activo.Value);

            query = query.OrderBy(u => u.Apellidos).ThenBy(u => u.Nombre);

            var pagedResults = await PaginatedList<Usuario>.CreateAsync(query, filtro.Page, filtro.PageSize);

            var vm = new UsuarioIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedResults
            };

            return View(vm);
        }
        //public async Task<IActionResult> Index()
        //{
        //    var usuarios = await _context.Usuarios.OrderBy(u => u.Apellidos).ThenBy(u => u.Nombre).ToListAsync();
        //    return View(usuarios);
        //}

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            var vm = BuildUsuarioVM();
            return View(vm);
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioVM vm)
        {
            // Para la creación, la nueva contraseña es obligatoria.
            if (string.IsNullOrEmpty(vm.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "La contraseña es obligatoria.");
            }

            // Verifica si el correo electrónico ya está registrado en la base de datos.
            if (await _context.Usuarios.AnyAsync(u => u.CorreoElectronico == vm.Usuario.CorreoElectronico))
            {
                // Si el correo ya existe, agrega un error al ModelState.
                ModelState.AddModelError("Usuario.CorreoElectronico", "Este correo electrónico ya está registrado.");
            }

            // Verifica si la CURP ya está registrada en la base de datos.
            if (await _context.Usuarios.AnyAsync(u => u.CURP == vm.Usuario.CURP))
            {
                ModelState.AddModelError("Usuario.CURP", "Esta CURP ya está registrada.");
            }

            // Removemos el error de validación del campo Password de la entidad,
            ModelState.Remove("Usuario.Password");

            
            if (ModelState.IsValid)
            {
                // Mapeo: Transfiere y hashea la nueva contraseña a la entidad.
                vm.Usuario.Password = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);

                _context.Add(vm.Usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargamos los datos del ViewModel y volvemos a la vista
            var reloadedVm = BuildUsuarioVM(vm.Usuario);
            reloadedVm.ConfirmPassword = vm.ConfirmPassword; // Mantener el valor para que el usuario no lo reescriba
            return View(reloadedVm);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            var vm = BuildUsuarioVM(usuario);
            return View(vm);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioVM vm)
        {
            if (id != vm.Usuario.Id) return BadRequest();

            // Solo validamos las contraseñas si el usuario escribió algo
            if (string.IsNullOrEmpty(vm.NewPassword))
            {
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");
            }

            // Verifica si el correo electrónico ya está registrado en otro usuario
            if (await _context.Usuarios.AnyAsync(u => u.CorreoElectronico == vm.Usuario.CorreoElectronico && u.Id != vm.Usuario.Id))
            {
                ModelState.AddModelError("Usuario.CorreoElectronico", "Este correo electrónico ya está en uso por otro usuario.");
            }

            // Verifica si la CURP ya está registrada en otro usuario
            if (await _context.Usuarios.AnyAsync(u => u.CURP == vm.Usuario.CURP && u.Id != vm.Usuario.Id))
            {
                ModelState.AddModelError("Usuario.CURP", "Esta CURP ya está en uso por otro usuario.");
            }

            // Removemos el error de validación del campo Password de la entidad,
            // ya que su valor se gestiona a través de NewPassword
            ModelState.Remove("Usuario.Password");

            if (ModelState.IsValid)
            {
                try
                {
                    // Mapeo: Solo si se proporcionó una nueva contraseña, la transferimos y hasheamos
                    if (!string.IsNullOrEmpty(vm.NewPassword))
                    {
                        vm.Usuario.Password = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);
                        _context.Update(vm.Usuario);
                    }
                    else
                    {
                        // Si no hay contraseña nueva, actualizamos el resto de los campos sin tocar la existente
                        _context.Entry(vm.Usuario).State = EntityState.Modified;
                        _context.Entry(vm.Usuario).Property(u => u.Password).IsModified = false;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Manejo de concurrencia
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var reloadedVm = BuildUsuarioVM(vm.Usuario);
            return View(reloadedVm);
        }


        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
