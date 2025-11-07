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
        // Esta es la "llave secreta" (reutilizable)
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;

        // CONSTRUCTOR: Prepara la llave secreta una sola vez
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
            };

            // FIRMA: Credenciales con la llave secreta y el algoritmo más fuerte
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // DESCRIPTOR: El "plano" del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),       // El payload
                Expires = DateTime.Now.AddMinutes(20),      // Expira en 20 minuto
                NotBefore = DateTime.Now,                   // Válido desde ahora
                IssuedAt = DateTime.Now,                    // Emitido ahora
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
