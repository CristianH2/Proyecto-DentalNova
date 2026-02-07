using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using System.Text.Json;

namespace Proyecto_DentalNova.Services
{
    public class AuthServiceApi : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthServiceApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            // Enviamos la petición POST al API
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", dto);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<LoginResponseDto>(options);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                string mensajeError = "Credenciales inválidas.";

                try
                {
                    // Propiedad "message" del JSON
                    var errorJson = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    if (errorJson.TryGetProperty("message", out var msg))
                    {
                        mensajeError = msg.GetString();
                    }
                }
                catch
                {
                    
                }

                throw new Exception(mensajeError);
            }
        }
    }
}
