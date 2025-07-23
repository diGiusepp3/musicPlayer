using MuziekApp.Views;

namespace MuziekApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registreer routes
        Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
    }
}