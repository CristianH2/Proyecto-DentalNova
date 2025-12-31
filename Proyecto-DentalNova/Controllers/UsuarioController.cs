using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces; // Para IUsuarioService
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_DentalNova.Models.UsuarioViewModel;

namespace Proyecto_DentalNova.Controllers
{
    // [Authorize(Roles = "Administrador")] 
    public class UsuarioController : Controller
    {
        // CAMBIO: Inyectamos el Servicio API, no el UnitOfWork
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Método auxiliar para construir el ViewModel
        private UsuarioVM BuildUsuarioVM(UsuarioAdminDtoIn? usuarioDto = null)
        {
            var vm = new UsuarioVM
            {
                // Si es null (Create), inicializamos uno nuevo activo.
                Usuario = usuarioDto ?? new UsuarioAdminDtoIn { Activo = true },

                // Listas para la UI (Dropdowns y Checkboxes)
                Generos = new List<SelectListItem>
                {
                    new SelectListItem("Masculino", "M"),
                    new SelectListItem("Femenino", "F"),
                    new SelectListItem("Otro", "O")
                },

                RolesDisponibles = new List<SelectListItem>
                {
                    new SelectListItem("Paciente", "Paciente"),
                    new SelectListItem("Odontologo", "Odontologo"),
                    new SelectListItem("Administrador", "Administrador")
                }
            };

            // Como en el DTO los roles ya vienen como List<string> ["Admin", "Paciente"],
            // podemos asignarlos directamente a RolesSeleccionados.
            if (usuarioDto != null && usuarioDto.Roles != null)
            {
                vm.RolesSeleccionados = usuarioDto.Roles;
            }

            return vm;
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

        // --- GET: Index (Listar) ---
        [HttpGet]
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] UsuarioFilterViewModel filtro)
        {
            HydrateFilter(filtro);

            // 1. Mapear ViewModel -> DTO de Filtro
            var filtroDto = new UsuarioFilterDto
            {
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                Id = filtro.Id,
                NombreLike = filtro.NombreLike,
                ApellidosLike = filtro.ApellidosLike,
                CorreoLike = filtro.CorreoLike,
                TelefonoLike = filtro.TelefonoLike,
                Genero = filtro.Genero,
                Activo = filtro.Activo
            };

            // 2. LLAMADA A LA API (Devuelve PagedResultDto)
            var apiResult = await _usuarioService.ObtenerUsuariosAsync(filtroDto);

            // 3. Convertir respuesta API -> PaginatedList para la Vista
            //    (Usamos el método Create que hicimos en el Paso 1)
            var pagedList = PaginatedList<UsuarioAdminDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                apiResult.PageIndex,
                filtro.PageSize);

            var vm = new UsuarioIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedList
            };

            return View(vm);
        }

        // --- GET: Details ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // LLAMADA A LA API
            var dto = await _usuarioService.ObtenerUsuarioPorIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // --- GET: Create ---
        public IActionResult Create()
        {
            var vm = BuildUsuarioVM();
            return View(vm);
        }

        // --- POST: Create ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioVM vm)
        {
            if (string.IsNullOrEmpty(vm.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "La contraseña es obligatoria.");
            }

            // Ignoramos validación de Password interno del DTO (si la tuviera)
            ModelState.Remove("Usuario.Password");

            if (ModelState.IsValid)
            {
                try
                {
                    // Preparamos el DTO directamente del ViewModel
                    var dtoIn = vm.Usuario;

                    // Asignamos los campos extra que están fuera de 'vm.Usuario'
                    dtoIn.Password = vm.NewPassword;
                    dtoIn.Roles = vm.RolesSeleccionados;

                    // Llama API
                    await _usuarioService.CrearUsuarioAsync(dtoIn);

                    // TempData para un mensaje "Toast"
                    TempData["MensajeExito"] = "Usuario creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    // ERROR DE API: Lo agregamos al resumen de validación del formulario
                    // string.Empty significa que es un error global del formulario, no de un campo específico
                    ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Error de conexión con el servidor.");
                }
            }

            var reloadedVm = BuildUsuarioVM(vm.Usuario);
            reloadedVm.ConfirmPassword = vm.ConfirmPassword;
            TempData["MensajeError"] = "Hubo un problema al validar los datos.";
            return View(reloadedVm);
        }

        // --- GET: Edit ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Llama API
            var dto = await _usuarioService.ObtenerUsuarioPorIdAsync(id.Value);
            if (dto == null) return NotFound();

            // Reconstruimos el usuario para el ViewModel
            var dtoIn = new UsuarioAdminDtoIn
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                CorreoElectronico = dto.CorreoElectronico,
                CURP = dto.CURP,
                Telefono = dto.Telefono,
                FechaNacimiento = dto.FechaNacimiento,
                Genero = dto.Genero,
                Activo = dto.Activo,
                Roles = dto.Roles
            };

            var vm = BuildUsuarioVM(dtoIn);
            return View(vm);
        }

        // --- POST: Edit ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioVM vm)
        {
            if (id != vm.Usuario.Id) return BadRequest();

            if (string.IsNullOrEmpty(vm.NewPassword))
            {
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");
            }
            ModelState.Remove("Usuario.Password");

            if (ModelState.IsValid)
            {
                try
                {
                    // Preparamos el DTO
                    var dtoIn = vm.Usuario;
                    dtoIn.Password = vm.NewPassword; // Puede ser null
                    dtoIn.Roles = vm.RolesSeleccionados;

                    // LLAMADA A LA API
                    await _usuarioService.ActualizarUsuarioAsync(id, dtoIn);
                    TempData["MensajeExito"] = "Usuario actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                }
            }

            var reloadedVm = BuildUsuarioVM(vm.Usuario);
            TempData["MensajeError"] = "Hubo un problema al validar los datos.";
            return View(reloadedVm);
        }

        // --- GET: Delete ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // Llama API
            var dto = await _usuarioService.ObtenerUsuarioPorIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // --- POST: Delete ---
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Llama API
            await _usuarioService.EliminarUsuarioAsync(id);
            TempData["MensajeExito"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // --- AJAX: Get Fecha Nacimiento ---
        [HttpGet]
        public async Task<IActionResult> GetUsuarioFechaNacimiento(int id)
        {
            // Llama API
            var fechaString = await _usuarioService.ObtenerFechaNacimientoAsync(id);
            return Json(new { fechaNacimiento = fechaString });
        }
    }
}