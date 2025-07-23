using Microsoft.Maui.Controls;

namespace MuziekApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // NavigationPage zodat PushAsync werkt
        MainPage = new NavigationPage(new MainPage())
        {
            BarBackgroundColor = Colors.Transparent,
            BarTextColor = Colors.White
        };
    }
}