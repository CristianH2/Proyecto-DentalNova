using DentalNova.Movil.ViewModels;

namespace DentalNova.Movil.Pages;

public partial class NotificacionesPage : ContentPage
{
    private readonly NotificacionesViewModel _viewModel;

    public NotificacionesPage(NotificacionesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearing();
    }
}