using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
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
        public IActionResult Login()
        {
            // Verifica si el usuario ya está logueado
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(InicioDeSesionDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            // Llamar a la API para obtener el Token
            var tokenDto = await _authService.LoginAsync(dto);

            if (tokenDto == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(dto);
            }

            // Decodificar el Token para leer los Claims (Roles, Nombre, Id)
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenDto.Token);

            // Crear los Claims para la Cookie
            var claims = new List<Claim>();

            // Copiamos los claims importantes del Token a la Cookie
            // (Así User.IsInRole("Administrador") funcionará en las Vistas MVC)
            claims.AddRange(jwtToken.Claims);

            // Guardamos el Token JWT CRUDO en un claim especial para llamar a la API después
            claims.Add(new Claim("JWT_TOKEN", tokenDto.Token));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Iniciar Sesión (Crear la Cookie)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = tokenDto.Expiracion // Sincronizar expiración
                });

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
