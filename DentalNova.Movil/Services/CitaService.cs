using DentalNova.Movil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace DentalNova.Movil.Services
{
    public class CitaService
    {
        private readonly HttpClient _httpClient;
        private readonly ConfiguracionService _config;

        public CitaService(HttpClient httpClient, ConfiguracionService config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<CitaDto>> ObtenerMisCitasAsync()
        {
            try
            {
                // Token Auth
                if (!string.IsNullOrEmpty(_config.Token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _config.Token);
                }

                int pacienteId = _config.PacienteId;

                // Construir URL con Query Params
                string url = $"api/Citas?PacienteId={pacienteId}&Page=1&PageSize=100";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedResponse<CitaDto>>();

                    // Retornamos solo la lista de items
                    return paginatedResult?.Items ?? new List<CitaDto>();
                }

                return new List<CitaDto>();
            }
            catch (Exception ex)
            {
                return new List<CitaDto>();
            }
        }

        public async Task<bool> AgendarCitaAsync(CitaCreateDto cita)
        {
            try
            {
                if (!string.IsNullOrEmpty(_config.Token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.Token);

                var response = await _httpClient.PostAsJsonAsync("api/Citas", cita);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error agendando: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CancelarCitaAsync(int citaId)
        {
            try
            {
                if (!string.IsNullOrEmpty(_config.Token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.Token);

                var content = JsonContent.Create(3);

                var response = await _httpClient.PatchAsync($"api/Citas/{citaId}/estatus", content);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

    }
}
