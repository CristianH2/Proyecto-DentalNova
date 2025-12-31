using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Services
{
    public class UsuarioServiceApi : IUsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Inyectamos HttpClient configurado y el contexto HTTP (para leer cookies/tokens más tarde)
        public UsuarioServiceApi(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // --- LISTAR CON FILTROS ---
        public async Task<PagedResultDto<UsuarioAdminDto>> ObtenerUsuariosAsync(UsuarioFilterDto filtro)
        {
            // Constrtuye la URL con los parámetros del filtro (Query String)
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            if (filtro.Id.HasValue) queryParams.Add("Id", filtro.Id.ToString());
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike)) queryParams.Add("NombreLike", filtro.NombreLike);
            if (!string.IsNullOrWhiteSpace(filtro.ApellidosLike)) queryParams.Add("ApellidosLike", filtro.ApellidosLike);
            if (!string.IsNullOrWhiteSpace(filtro.CorreoLike)) queryParams.Add("CorreoLike", filtro.CorreoLike);
            if (!string.IsNullOrWhiteSpace(filtro.TelefonoLike)) queryParams.Add("TelefonoLike", filtro.TelefonoLike);
            if (filtro.Genero.HasValue) queryParams.Add("Genero", filtro.Genero.ToString());
            if (filtro.Activo.HasValue) queryParams.Add("Activo", filtro.Activo.ToString());

            var url = QueryHelpers.AddQueryString("api/Usuarios", queryParams);

            // Llama al GET
            await AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Lanza excepción si no es 200 OK

            // Leer el JSON y devolverlo
            return await response.Content.ReadFromJsonAsync<PagedResultDto<UsuarioAdminDto>>();
        }

        // --- OBTENER POR ID ---
        public async Task<UsuarioAdminDto> ObtenerUsuarioPorIdAsync(int id)
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<UsuarioAdminDto>($"api/Usuarios/{id}");
        }

        // --- CREAR ---
        public async Task CrearUsuarioAsync(UsuarioAdminDtoIn dto)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Usuarios", dto);

            if (!response.IsSuccessStatusCode)
            {
                // Leemos el contenido del error (el JSON que devuelve la API)
                var errorJson = await response.Content.ReadAsStringAsync();

                string mensajeError = "Ocurrió un error al procesar la solicitud.";

                try
                {
                    // Intentamos parsear el JSON (asumiendo que la API devuelve { "mensaje": "..." })
                    // Usamos Newtonsoft o System.Text.Json
                    var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(errorJson);
                    if (errorObj != null && errorObj.ContainsKey("mensaje"))
                    {
                        mensajeError = errorObj["mensaje"];
                    }
                }
                catch
                {
                    // Si no es JSON, usamos el texto crudo o un genérico
                    if (!string.IsNullOrEmpty(errorJson)) mensajeError = errorJson;
                }

                // 3. Lanzamos una excepción con el mensaje LIMPIO
                throw new HttpRequestException(mensajeError);
            }
        }

        // --- ACTUALIZAR ---
        public async Task ActualizarUsuarioAsync(int id, UsuarioAdminDtoIn dto)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Usuarios/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        // --- ELIMINAR ---
        public async Task EliminarUsuarioAsync(int id)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Usuarios/{id}");
            response.EnsureSuccessStatusCode();
        }

        // --- AUXILIAR FECHA ---
        public async Task<string> ObtenerFechaNacimientoAsync(int id)
        {
            await AddAuthorizationHeader();
            // La API devuelve un objeto { fechaNacimiento: "yyyy-mm-dd" }
            var resultado = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>($"api/Usuarios/check-birthdate/{id}");
            return resultado?["fechaNacimiento"];
        }

        // --- MÉTODO PRIVADO PARA EL TOKEN ---
        private async Task AddAuthorizationHeader()
        {
            // Acceder al usuario actual a través del HttpContext
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null && user.Identity.IsAuthenticated)
            {
                // Buscar el claim "JWT_TOKEN" que guardamos en el AccountController
                var token = user.FindFirst("JWT_TOKEN")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    // Poner el token en el Header Authorization
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }
    }
}
