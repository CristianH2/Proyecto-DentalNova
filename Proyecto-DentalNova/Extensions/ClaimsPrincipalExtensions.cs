using System.Security.Claims;

namespace Proyecto_DentalNova.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetOdontologoId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("OdontologoId");

            if (claim != null && int.TryParse(claim.Value, out int id))
            {
                return id;
            }

            return null;
        }

        public static int? GetPacienteId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("PacienteId");

            if (claim != null && int.TryParse(claim.Value, out int id))
            {
                return id;
            }

            return null;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out int id) ? id : 0;
        }
    }
}
