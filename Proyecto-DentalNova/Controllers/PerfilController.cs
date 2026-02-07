using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_DentalNova.Extensions;
using Proyecto_DentalNova.Models.PerfilViewModel;

namespace Proyecto_DentalNova.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public PerfilController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Perfil
        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            // 2. Obtener datos del usuario desde el servicio
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(userId);
            if (usuario == null) return NotFound();

            // 3. Mapear al ViewModel
            var vm = new PerfilViewModel
            {
                Id = usuario.Id,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellidos}",
                Correo = usuario.CorreoElectronico,
                Telefono = usuario.Telefono,
                Rol = usuario.Roles?.FirstOrDefault() ?? "Usuario",
                FechaNacimiento = usuario.FechaNacimiento?.ToString("dd/MM/yyyy")
            };

            return View(vm);
        }

        // POST: Perfil/CambiarPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(PerfilViewModel model)
        {
            // Ignoramos validaciones de campos de lectura
            ModelState.Remove("NombreCompleto");
            ModelState.Remove("Correo");
            ModelState.Remove("Rol");
            ModelState.Remove("FechaRegistro");
            ModelState.Remove("Telefono");

            if (!ModelState.IsValid)
            {
                return await RecargarVista(model);
            }

            try
            {
                var userId = User.GetUserId();

                // Llamada al servicio para cambiar contraseña
                await _usuarioService.CambiarContrasenaAsync(userId, model.PasswordActual, model.PasswordNuevo);

                TempData["MensajeExito"] = "Contraseña actualizada correctamente.";

                // Limpiamos los campos de password
                model.PasswordActual = "";
                model.PasswordNuevo = "";
                model.PasswordConfirmacion = "";

                return await RecargarVista(model);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cambiar contraseña";
                return await RecargarVista(model);
            }
        }

        // Helper
        private async Task<IActionResult> RecargarVista(PerfilViewModel model)
        {
            var userId = User.GetUserId();
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(userId);

            if (usuario != null)
            {
                model.Id = usuario.Id;
                model.NombreCompleto = $"{usuario.Nombre} {usuario.Apellidos}";
                model.Correo = usuario.CorreoElectronico;
                model.Telefono = usuario.Telefono;
                model.Rol = usuario.Roles?.FirstOrDefault() ?? "Usuario";
                model.FechaNacimiento = usuario.FechaNacimiento?.ToString("dd/MM/yyyy");
            }

            return View("Index", model);
        }
    }
}
