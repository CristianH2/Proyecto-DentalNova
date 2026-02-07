using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Handlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null)
            {
                // Recupera el Token de la Sesión
                var token = context.Session.GetString("Token");

                // Si existe el token, lo agregamos a la cabecera Authorization
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            // Continuamos con la petición normal
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
