namespace MuziekApp.Views;

public partial class MainView : ContentPage
{
    public MainView()
    {
        InitializeComponent();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Navigatie terug naar loginpagina
        await Shell.Current.GoToAsync(nameof(LoginView));
    }
}