using Microsoft.Maui.Controls;
using MuziekApp.Views;

namespace MuziekApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        this.Loaded += async (s, e) =>
        {
            await LogoImage.FadeTo(1, 1500, Easing.CubicIn);
            await LogoImage.ScaleTo(1.05, 300, Easing.CubicOut);
            await LogoImage.ScaleTo(1, 300, Easing.CubicIn);

            await Task.Delay(1000);
            await AppNameLabel.FadeTo(1, 1000, Easing.CubicIn);

            await Task.Delay(1000);
            await DescriptionLabel.FadeTo(1, 1000, Easing.CubicIn);

            await Task.Delay(500);
            await CreateAccountButton.FadeTo(1, 800, Easing.CubicIn);
            await LoginButton.FadeTo(1, 800, Easing.CubicIn);
        };
    }

    private async Task AnimatePageOut()
    {
        await this.TranslateTo(-this.Width, 0, 400, Easing.CubicIn);
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        await AnimatePageOut();
        await Navigation.PushAsync(new RegisterView());
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await AnimatePageOut();
        await Navigation.PushAsync(new LoginView());
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.TranslationX = 0; // Reset naar startpositie
    }
}