using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using MuziekApp.Views;

namespace MuziekApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // Check of gebruiker al eerder was ingelogd
        var userId = Preferences.Get("user_id", 0);
        if (userId > 0)
        {
            // Gebruiker bestaat → direct naar MainView
            await Shell.Current.GoToAsync($"//{nameof(MainView)}");
        }
        else
        {
            // Geen gebruiker → naar loginpagina
            await Shell.Current.GoToAsync($"//{nameof(LoginView)}");
        }
    }
}