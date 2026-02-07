using DentalNova.Movil.ViewModels;

namespace DentalNova.Movil.Pages;

public partial class AgendarPage : ContentPage
{
    private readonly AgendarViewModel _viewModel;

    public AgendarPage(AgendarViewModel viewModel)
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