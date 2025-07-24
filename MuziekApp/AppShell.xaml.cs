using MuziekApp.Views;
using Microsoft.Maui.Storage;

namespace MuziekApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Shell.SetNavBarIsVisible(this, false);

        // Registreer routes
        Routing.RegisterRoute(nameof(MainView), typeof(MainView));
        Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        
        
        var userId = Preferences.Get("user_id", 0); 
        if (userId == 2 || userId == 3 || userId == 4)
        {
            Routing.RegisterRoute(nameof(UploadSongView), typeof(UploadSongView));
        }
    }
}