using DentalNova.Core.Repository.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentalNova.Security
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); //llave desde appsettings.json
        }

        public string GenerarToken(Usuario usuario)
        {
            // PAYLOAD: Lista de información (claims) que va dentro del token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.CorreoElectronico),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellidos}") // Nombre para mostrar
            };

            // Agregamos un Claim por cada rol que tenga el usuario
            if (usuario.Roles != null && usuario.Roles.Any())
            {
                foreach (var rol in usuario.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol.Nombre));
                }
            }

            // FIRMA: Credenciales con la llave secreta
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // DESCRIPTOR: El "plano" del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),       // El payload
                Expires = DateTime.UtcNow.AddHours(4),      // Expira en 20 minuto
                NotBefore = DateTime.UtcNow,                // Válido desde ahora
                IssuedAt = DateTime.UtcNow,                 // Emitido ahora
                SigningCredentials = creds,                 // La firma
                Issuer = _config["Jwt:Issuer"],             // Quién lo emite (la app)
                Audience = _config["Jwt:Audience"]          // Para quién es (la app)
            };

            // CREACIÓN: Ensambla y escribe el token como un string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
