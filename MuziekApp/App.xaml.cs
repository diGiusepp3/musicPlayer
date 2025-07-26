using MuziekApp.Services;
using MuziekApp.Views;

namespace MuziekApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        LocalStorageService.Initialize();

        MainPage = new AppShell();

        // Dispatcher gebruiken zodat Shell klaar is
        MainPage.Dispatcher.Dispatch(async () => await HandleStartupNavigation());
    }

    private async Task HandleStartupNavigation()
    {
        try
        {
            var savedUser = LocalStorageService.LoadUser();
            if (savedUser != null)
            {
                Console.WriteLine($"[USER] {savedUser.DisplayName} is al ingelogd → Direct naar MainView");
                // **Absolute route naar MainView**
                await Shell.Current.GoToAsync($"//{nameof(MainView)}", true);
            }
            else
            {
                Console.WriteLine("[USER] Geen user.json gevonden → Naar LoginView");
                // **Absolute route naar LoginView**
                await Shell.Current.GoToAsync($"//{nameof(LoginView)}", true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Navigatie fout: " + ex.Message);
        }
    }
}