using DentalNova.AppMovil.Services;
using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DentalNova.AppMovil.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService;
            RegisterCommand = new Command(OnRegisterClicked);
            LoginCommand = new Command(async () => await Application.Current.MainPage.Navigation.PopAsync());
        }

        // --- Usuario---
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string CorreoElectronico { get; set; }
        public string Curp { get; set; }
        public string Telefono { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public DateTime FechaNacimiento { get; set; } = DateTime.Today.AddYears(-18);

        // Picker de Género
        public List<string> Generos { get; } = new List<string> { "Masculino", "Femenino" };
        public string GeneroSeleccionado { get; set; }

        // --- Perfil Paciente ---
        // Usamos SetProperty para notificar a la vista y mostrar/ocultar los campos de texto

        private bool _conAlergias;
        public bool ConAlergias
        {
            get => _conAlergias;
            set => SetProperty(ref _conAlergias, value);
        }
        public string Alergias { get; set; }

        private bool _conCronicas;
        public bool ConCronicas
        {
            get => _conCronicas;
            set => SetProperty(ref _conCronicas, value);
        }
        public string EnfermedadesCronicas { get; set; }

        private bool _conMedicamentos;
        public bool ConMedicamentos
        {
            get => _conMedicamentos;
            set => SetProperty(ref _conMedicamentos, value);
        }
        public string MedicamentosActuales { get; set; }

        private bool _conAntecedentes;
        public bool ConAntecedentes
        {
            get => _conAntecedentes;
            set => SetProperty(ref _conAntecedentes, value);
        }
        public string AntecedentesFamiliares { get; set; }

        // Comandos
        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        private async void OnRegisterClicked()
        {
            if (IsBusy) return;

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellidos) || string.IsNullOrWhiteSpace(CorreoElectronico))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Complete los campos obligatorios.", "OK");
                return;
            }
            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Curp) || Curp.Length != 18)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "La CURP debe tener 18 caracteres.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                // Construir el DTO
                var registroDto = new RegistroCompletoDto
                {
                    Usuario = new UsuarioDtoIn
                    {
                        Nombre = this.Nombre,
                        Apellidos = this.Apellidos,
                        CorreoElectronico = this.CorreoElectronico,
                        CURP = this.Curp.ToUpper(), // Aseguramos mayúsculas
                        Password = this.Password,
                        Telefono = this.Telefono,
                        FechaNacimiento = this.FechaNacimiento,
                        // Convertir selección del Picker a char
                        Genero = string.IsNullOrEmpty(GeneroSeleccionado) ? null : (GeneroSeleccionado == "Masculino" ? 'M' : 'F')
                    },
                    Paciente = new PerfilPacienteDtoIn
                    {
                        ConAlergias = this.ConAlergias,
                        // Enviamos el texto solo si el checkbox es true, si no, null (limpieza de datos)
                        Alergias = this.ConAlergias ? this.Alergias : null,

                        ConEnfermedadesCronicas = this.ConCronicas,
                        EnfermedadesCronicas = this.ConCronicas ? this.EnfermedadesCronicas : null,

                        ConMedicamentosActuales = this.ConMedicamentos,
                        MedicamentosActuales = this.ConMedicamentos ? this.MedicamentosActuales : null,

                        ConAntecedentesFamiliares = this.ConAntecedentes,
                        AntecedentesFamiliares = this.ConAntecedentes ? this.AntecedentesFamiliares : null
                    }
                };

                // Enviar a la API
                var response = await _apiService.PostAsync<RegistroCompletoDto, object>("Auth/RegisterCompleto", registroDto);

                if (response != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Cuenta creada correctamente. Por favor inicie sesión.", "OK");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    // El ApiService suele capturar el error genérico, pero aquí podrías mejorar
                    // para mostrar el mensaje exacto que devuelve tu API ("La CURP ya existe", etc.)
                    await Application.Current.MainPage.DisplayAlert("Atención", "No se pudo registrar. Verifique que el correo o CURP no existan ya.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
