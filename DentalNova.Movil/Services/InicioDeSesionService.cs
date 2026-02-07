using DentalNova.Movil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace DentalNova.Movil.Services
{
    public class InicioDeSesionService
    {
        private readonly HttpClient _httpClient;
        public InicioDeSesionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponseDto> IniciarSesionAsync(string correo, string password)
        {
            try
            {
                var loginData = new LoginRequestDto
                {
                    Correo = correo,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> RegistrarUsuarioAsync(RegistroCompletoDto registro)
        {
            try
            {
                // Este endpoint es público, no requiere Token
                var response = await _httpClient.PostAsJsonAsync("api/Auth/RegistroCompleto", registro);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registro: {ex.Message}");
                return false;
            }
        }
    }
}
