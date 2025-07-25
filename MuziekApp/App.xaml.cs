using MuziekApp.Services;
using MuziekApp.Views;

namespace MuziekApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        LocalStorageService.Initialize();

        // Start navigatie pas nadat de UI klaar is
        MainPage.NavigatedTo += async (s, e) => await HandleStartupNavigation();
    }

    private async Task HandleStartupNavigation()
    {
        try
        {
            var savedUser = LocalStorageService.LoadUser();
            if (savedUser != null)
            {
                Console.WriteLine($"[USER] {savedUser.DisplayName} is al ingelogd → Direct naar MainView");
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync($"{nameof(MainView)}");
            }
            else
            {
                Console.WriteLine("[USER] Geen user.json gevonden → Naar LoginView");
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync($"//{nameof(LoginView)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Navigatie fout: " + ex.Message);
        }
        finally
        {
            // Zorg dat dit maar één keer gebeurt
            MainPage.NavigatedTo -= async (s, e) => await HandleStartupNavigation();
        }
    }
}