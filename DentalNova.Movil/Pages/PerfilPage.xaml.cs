using DentalNova.Movil.ViewModels;

namespace DentalNova.Movil.Pages;

public partial class PerfilPage : ContentPage
{
    private readonly PerfilViewModel _viewModel;

    public PerfilPage(PerfilViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel != null)
        {
            await _viewModel.OnAppearing();
        }
    }
}