using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using MuziekApp.Views;
using MuziekApp.Models;

namespace MuziekApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        [ObservableProperty] private string email;
        [ObservableProperty] private string password;
        [ObservableProperty] private string message;

        public LoginViewModel(DatabaseService database)
        {
            _database = database;
        }

        [RelayCommand]
        private async Task Login()
        {
            Message = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Vul alle velden in.";
                return;
            }

            var (user, serverMessage) = await _database.LoginAndGetUserAsync(Email, Password);
            if (user == null)
            {
                Message = serverMessage ?? "Verkeerde gegevens!";
                return;
            }

            try
            {
                LocalStorageService.SaveUser(user);
            }
            catch (Exception ex)
            {
                Message = "Opslaan van gebruiker mislukt: " + ex.Message;
                return;
            }

            Message = $"Welkom {user.DisplayName ?? user.Email}!";
            await Task.Delay(1000);
            await Shell.Current.GoToAsync($"//{nameof(MainView)}");
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterView));
        }
    }
}