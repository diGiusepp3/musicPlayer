using MuziekApp.Views;

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
        Routing.RegisterRoute(nameof(UploadSongView), typeof(UploadSongView));
        Routing.RegisterRoute(nameof(VideoDownloadView), typeof(VideoDownloadView));

    }
} 