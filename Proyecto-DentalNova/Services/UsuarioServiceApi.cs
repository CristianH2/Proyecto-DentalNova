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

        public UsuarioServiceApi(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

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

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PagedResultDto<UsuarioAdminDto>>();
        }

        public async Task<UsuarioAdminDto> ObtenerUsuarioPorIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<UsuarioAdminDto>($"api/Usuarios/{id}");
        }

        public async Task CrearUsuarioAsync(UsuarioAdminDtoIn dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Usuarios", dto);

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();

                string mensajeError = "Ocurrió un error al procesar la solicitud.";

                try
                {
                    // Parsear el JSON 
                    var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(errorJson);
                    if (errorObj != null && errorObj.ContainsKey("mensaje"))
                    {
                        mensajeError = errorObj["mensaje"];
                    }
                }
                catch
                {
                    if (!string.IsNullOrEmpty(errorJson)) mensajeError = errorJson;
                }

                throw new HttpRequestException(mensajeError);
            }
        }

        public async Task ActualizarUsuarioAsync(int id, UsuarioAdminDtoIn dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Usuarios/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task EliminarUsuarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Usuarios/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> ObtenerFechaNacimientoAsync(int id)
        {
            // La API devuelve un objeto { fechaNacimiento: "yyyy-mm-dd" }
            var resultado = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>($"api/Usuarios/check-birthdate/{id}");
            return resultado?["fechaNacimiento"];
        }

        public async Task CambiarContrasenaAsync(int userId, string passwordActual, string passwordNuevo)
        {
            var dto = new CambioPasswordDtoIn
            {
                PasswordActual = passwordActual,
                PasswordNuevo = passwordNuevo
            };

            // Asumiendo que usas HttpClient
            var response = await _httpClient.PostAsJsonAsync("api/Usuarios/cambiar-password", dto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al cambiar contraseña: {errorContent}");
            }
        }
    }
}
