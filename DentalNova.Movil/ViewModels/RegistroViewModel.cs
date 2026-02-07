using DentalNova.Movil.Models;
using DentalNova.Movil.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.ViewModels
{
    public class RegistroViewModel : BaseViewModel
    {
        private readonly InicioDeSesionService _authService;

        // --- Datos ---
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Curp { get; set; }
        public DateTime FechaNacimiento { get; set; } = DateTime.Today.AddYears(-18); // Default 18 años
        public string Genero { get; set; }

        // --- Cuenta ---
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // --- Médicos ---

        private bool conAlergias;
        public bool ConAlergias { get => conAlergias; set { conAlergias = value; OnPropertyChanged(); } }
        public string Alergias { get; set; }

        private bool conEnfermedades;
        public bool ConEnfermedades { get => conEnfermedades; set { conEnfermedades = value; OnPropertyChanged(); } }
        public string Enfermedades { get; set; }

        private bool conMedicamentos;
        public bool ConMedicamentos { get => conMedicamentos; set { conMedicamentos = value; OnPropertyChanged(); } }
        public string Medicamentos { get; set; }

        private bool conAntecedentes;
        public bool ConAntecedentes { get => conAntecedentes; set { conAntecedentes = value; OnPropertyChanged(); } }
        public string Antecedentes { get; set; }

        public string Observaciones { get; set; }


        public Command RegistrarCommand { get; }
        public Command CancelarCommand { get; }

        public RegistroViewModel(InicioDeSesionService authService)
        {
            _authService = authService;
            Title = "Crear Cuenta";

            RegistrarCommand = new Command(OnRegistrar);
            CancelarCommand = new Command(async () => await Application.Current.MainPage.Navigation.PopAsync());
        }

        private async void OnRegistrar()
        {
            if (IsBusy) return;

            // Validaciones
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Nombre, Correo y Contraseña son obligatorios", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                return;
            }

            IsBusy = true;

            // Armar el DTO

            var genero = "";
            if (Genero == "Femenino") genero = "F";
            else if (Genero == "Masculino") genero = "M";
            else genero = "O";

            var registroDto = new RegistroCompletoDto
            {
                Usuario = new UsuarioRegistroDto
                {
                    Nombre = Nombre,
                    Apellidos = Apellidos,
                    CorreoElectronico = Email,
                    Password = Password,
                    Telefono = Telefono,
                    Curp = Curp,
                    FechaNacimiento = FechaNacimiento,
                    Genero = genero
                },
                Paciente = new PacienteRegistroDto
                {
                    ConAlergias = ConAlergias,
                    Alergias = ConAlergias ? Alergias : "Ninguna", // Limpieza de datos

                    ConEnfermedadesCronicas = ConEnfermedades,
                    EnfermedadesCronicas = ConEnfermedades ? Enfermedades : "Ninguna",

                    ConMedicamentosActuales = ConMedicamentos,
                    MedicamentosActuales = ConMedicamentos ? Medicamentos : "Ninguno",

                    ConAntecedentesFamiliares = ConAntecedentes,
                    AntecedentesFamiliares = ConAntecedentes ? Antecedentes : "Ninguno",

                    Observaciones = Observaciones ?? ""
                }
            };

            // Enviar
            bool exito = await _authService.RegistrarUsuarioAsync(registroDto);

            IsBusy = false;

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Cuenta creada correctamente. Ahora inicia sesión.", "OK");
                // Regresar al Login
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar. Verifica tus datos (ej. correo duplicado).", "OK");
            }
        }
    }
}
