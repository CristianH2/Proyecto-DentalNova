using DentalNova.AppMovil.ViewModels;

namespace DentalNova.AppMovil.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}