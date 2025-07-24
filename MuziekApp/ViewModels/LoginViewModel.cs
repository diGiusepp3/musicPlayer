using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using MuziekApp.Views;

namespace MuziekApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string message = string.Empty;

        public LoginViewModel(DatabaseService database)
        {
            _database = database;
        }

        [RelayCommand]
        private async Task Login()
        {
            var user = await _database.LoginAndGetUserAsync(Email, Password);
            if (user == null)
            {
                Message = "Verkeerde gegevens!";
                return;
            }

            // Opslaan in lokaal JSON-bestand
            LocalStorageService.SaveUser(new UserData
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Premium = user.Premium
            });

            Message = $"Welkom {user.DisplayName}!";
            await Shell.Current.GoToAsync($"{nameof(MainView)}");
        }
    }
}