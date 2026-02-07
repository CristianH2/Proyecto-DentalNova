using DentalNova.Movil.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace DentalNova.Movil
{
    public partial class App : Application
    {
        public App(InicioDeSesionPage loginPage)
        {
            InitializeComponent();
            //MainPage = loginPage;
            MainPage = new NavigationPage(loginPage);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            //return new Window(new AppShell());
            return base.CreateWindow(activationState);
        }
    }
}