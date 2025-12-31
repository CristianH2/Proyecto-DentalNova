using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;

namespace Proyecto_DentalNova.Services
{
    public class AuthServiceApi : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthServiceApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenDto> LoginAsync(InicioDeSesionDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

            if (!response.IsSuccessStatusCode)
            {
                return null; // Login fallido
            }

            return await response.Content.ReadFromJsonAsync<TokenDto>();
        }
    }
}
