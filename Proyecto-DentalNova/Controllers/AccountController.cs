using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Proyecto_DentalNova.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Si ya está logueado
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var response = await _authService.LoginAsync(model);

                // Definimos los roles permitidos
                bool esPersonalMedico = response.Roles.Contains("Administrador") ||
                                        response.Roles.Contains("Odontologo");

                if (!esPersonalMedico)
                {
                    ViewBag.Error = "Acceso Denegado: Su perfil no tiene permisos para acceder al portal administrativo.";
                    return View(model);
                }

                // Construir los Claims
                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, response.NombreCompleto),
                                new Claim(ClaimTypes.NameIdentifier, response.UsuarioId.ToString()),
                                new Claim("Token", response.Token)
                            };

                if (response.Roles != null)
                {
                    foreach (var rol in response.Roles) claims.Add(new Claim(ClaimTypes.Role, rol));
                }

                // Si el usuario es paciente u odontólogo, agregar sus IDs
                if (response.PacienteId.HasValue)
                    claims.Add(new Claim("PacienteId", response.PacienteId.Value.ToString()));

                if (response.OdontologoId.HasValue)
                    claims.Add(new Claim("OdontologoId", response.OdontologoId.Value.ToString()));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(4)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                HttpContext.Session.SetString("Token", response.Token);
                if (response.PacienteId.HasValue) HttpContext.Session.SetInt32("PacienteId", response.PacienteId.Value);

                // Redirección
                if (response.Roles.Contains("Odontologo"))
                {
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Borrar Cookie
            HttpContext.Session.Clear(); // Borrar Sesión

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
