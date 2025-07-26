using MuziekApp.Services;

namespace MuziekApp.Views;

public partial class MainPage : ContentPage
{
    private readonly StartupCheckService _startupChecker;

    // Injectie via constructor
    public MainPage(StartupCheckService startupChecker)
    {
        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);

        _startupChecker = startupChecker;

        this.Loaded += async (s, e) =>
        {
            await LogoImage.FadeTo(1, 1500, Easing.CubicIn);
            await LogoImage.ScaleTo(1.05, 300, Easing.CubicOut);
            await LogoImage.ScaleTo(1, 300, Easing.CubicIn);
            await Task.Delay(1000);
            await AppNameLabel.FadeTo(1, 1000, Easing.CubicIn);
            await Task.Delay(1000);
            await Task.Delay(500);
            await CreateAccountButton.FadeTo(1, 800, Easing.CubicIn);
            await LoginButton.FadeTo(1, 800, Easing.CubicIn);
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        MainContainer.TranslationX = 0;

        await Task.Delay(500); 
        _ = _startupChecker?.RunCheckAsync(); // null-check toegevoegd
    }

    private async Task AnimatePageOut()
    {
        await MainContainer.TranslateTo(-this.Width, 0, 400, Easing.CubicIn);
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        await AnimatePageOut();
        await Shell.Current.GoToAsync(nameof(RegisterView));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await AnimatePageOut();
        await Shell.Current.GoToAsync(nameof(LoginView));
    }
}