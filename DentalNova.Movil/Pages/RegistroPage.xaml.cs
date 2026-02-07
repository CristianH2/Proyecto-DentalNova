using DentalNova.Movil.ViewModels;

namespace DentalNova.Movil.Pages;

public partial class RegistroPage : ContentPage
{
    public RegistroPage(RegistroViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}