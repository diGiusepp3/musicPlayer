using MuziekApp.ViewModels;

namespace MuziekApp.Views;

public partial class MainView : ContentPage
{
    public MainView(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Navigatie terug naar loginpagina
        await Shell.Current.GoToAsync(nameof(LoginView));
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetNavBarIsVisible(this, false);
    }

}