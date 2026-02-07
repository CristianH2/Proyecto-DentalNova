using DentalNova.Movil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace DentalNova.Movil.Services
{
    public class PerfilService
    {
        private readonly HttpClient _httpClient;
        private readonly ConfiguracionService _config;

        public PerfilService(HttpClient httpClient, ConfiguracionService config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<PerfilUsuarioDto> ObtenerUsuarioAsync()
        {
            return await GetAsync<PerfilUsuarioDto>($"api/Perfiles/obtener-usuario");
        }

        public async Task<PerfilPacienteDto> ObtenerPacienteAsync()
        {
            return await GetAsync<PerfilPacienteDto>($"api/Perfiles/obtener-paciente");
        }

        // Método genérico para no repetir código GET
        private async Task<T> GetAsync<T>(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(_config.Token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.Token);

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T>();

                return default;
            }
            catch { return default; }
        }

        public async Task<bool> CambiarPasswordAsync(string actual, string nuevo)
        {
            try
            {
                if (!string.IsNullOrEmpty(_config.Token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.Token);

                var dto = new CambiarPasswordDto
                {
                    PasswordActual = actual,
                    PasswordNuevo = nuevo
                };

                var response = await _httpClient.PostAsJsonAsync("api/Usuarios/cambiar-password", dto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
