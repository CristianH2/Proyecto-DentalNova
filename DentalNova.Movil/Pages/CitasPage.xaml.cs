using DentalNova.Movil.ViewModels;

namespace DentalNova.Movil.Pages;

public partial class CitasPage : ContentPage
{
    private readonly CitasViewModel _viewModel;

    public CitasPage(CitasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // Automático cada que se abre
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearing();
    }
}