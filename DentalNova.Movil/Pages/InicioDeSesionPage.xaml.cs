using DentalNova.Movil.ViewModels;

namespace DentalNova.Movil.Pages;

public partial class InicioDeSesionPage : ContentPage
{
    public InicioDeSesionPage(InicioDeSesionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        NavigationPage.SetHasNavigationBar(this, false);
    }
}