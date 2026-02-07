using DentalNova.Movil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace DentalNova.Movil.Services
{
    public class OdontologoService
    {
        private readonly HttpClient _httpClient;
        private readonly ConfiguracionService _config;

        public OdontologoService(HttpClient httpClient, ConfiguracionService config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<OdontologoDto>> GetOdontologosAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_config.Token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.Token);

                var response = await _httpClient.GetAsync("api/Odontologos?PageSize=100");
                if (response.IsSuccessStatusCode)
                {
                    var paginated = await response.Content.ReadFromJsonAsync<PaginatedResponse<OdontologoDto>>();
                    return paginated?.Items ?? new List<OdontologoDto>();
                }
                return new List<OdontologoDto>();
            }
            catch { return new List<OdontologoDto>(); }
        }

        public async Task<List<HorarioOdontologoDto>> GetHorariosAsync(int odontologoId)
        {
            try
            {
                if (!string.IsNullOrEmpty(_config.Token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.Token);

                var response = await _httpClient.GetAsync($"api/HorariosOdontologos/odontologo/{odontologoId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<HorarioOdontologoDto>>();
                }
                return new List<HorarioOdontologoDto>();
            }
            catch { return new List<HorarioOdontologoDto>(); }
        }
    }
}
