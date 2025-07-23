using System;
using Microsoft.Maui.Controls;

namespace MuziekApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Animaties starten zodra pagina geladen is
            this.Loaded += async (s, e) =>
            {
                // Logo fade-in
                await LogoImage.FadeTo(1, 1500, Easing.CubicIn);
                await LogoImage.ScaleTo(1.05, 300, Easing.CubicOut);
                await LogoImage.ScaleTo(1, 300, Easing.CubicIn);

                // Wacht 1 seconde -> App naam
                await Task.Delay(1000);
                await AppNameLabel.FadeTo(1, 1000, Easing.CubicIn);

                // Wacht 1 seconde -> Beschrijving
                await Task.Delay(1000);
                await DescriptionLabel.FadeTo(1, 1000, Easing.CubicIn);

                // Wacht 0,5 seconde -> knoppen
                await Task.Delay(500);
                await CreateAccountButton.FadeTo(1, 800, Easing.CubicIn);
                await LoginButton.FadeTo(1, 800, Easing.CubicIn);
            };
        }

        private void OnPlayClicked(object sender, EventArgs e)
        {
            DisplayAlert("Muziek App", "Muziek wordt afgespeeld!", "OK");
        }
    }
}