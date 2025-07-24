using Microsoft.Maui.Controls;

namespace MuziekApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}