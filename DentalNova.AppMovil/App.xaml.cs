using DentalNova.AppMovil.Helpers;
using DentalNova.AppMovil.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DentalNova.AppMovil
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Verificamos la sesión al iniciar
            CheckSession();
        }
        private void CheckSession()
        {
            // Leemos el token de forma síncrona para el arranque
            // Nota: SecureStorage.GetAsync es asíncrono, usamos Task.Run.Result con precaución en el constructor
            var token = Task.Run(async () => await SecureStorage.GetAsync(Constants.AuthTokenKey)).Result;

            if (string.IsNullOrEmpty(token))
            {
                // NO AUTENTICADO: Vamos al Login
                // Usamos el ServiceProvider para resolver LoginPage y sus dependencias (ViewModel -> Service)
                var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                MainPage = new NavigationPage(loginPage);
            }
            else
            {
                // AUTENTICADO: Vamos al Dashboard (Shell)
                MainPage = new AppShell();
            }
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
    }
}