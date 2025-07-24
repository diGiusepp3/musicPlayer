using MuziekApp.Services;
using MuziekApp.Views;

namespace MuziekApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        // Zorg dat de basis mappen bestaan (Listen!)
        LocalStorageService.Initialize();

        // Navigeer nadat Shell geladen is
        Application.Current.Dispatcher.Dispatch(async () =>
        {
            var savedUser = LocalStorageService.LoadUser();
            if (savedUser != null)
            {
                Console.WriteLine($"[USER] {savedUser.DisplayName} is al ingelogd → Direct naar MainView");
                await Shell.Current.GoToAsync($"{nameof(MainView)}");
                Console.WriteLine("[ROUTE] Navigatie uitgevoerd naar MainView");
            }
            else
            {
                Console.WriteLine("[USER] Geen user.json gevonden → Naar LoginView");
                await Shell.Current.GoToAsync($"//{nameof(LoginView)}");
                Console.WriteLine("[ROUTE] Navigatie uitgevoerd naar LoginView");
            }
        });
    }
}