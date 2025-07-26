using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using MuziekApp.Views;
using MuziekApp.Models;

namespace MuziekApp.ViewModels;

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
        try
        {
            Message = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Vul alle velden in.";
                return;
            }

            var user = await _database.LoginAndGetUserAsync(Email, Password);
            if (user == null)
            {
                Message = "Verkeerde gegevens!";
                return;
            }

            // Sla gebruiker lokaal op
            try
            {
                LocalStorageService.SaveUser(user);
            }
            catch (Exception ex)
            {
                Message = "Opslaan van gebruiker mislukt: " + ex.Message;
            }

            Message = $"Welkom {user.DisplayName}!";

            // Veilig navigeren
            if (Shell.Current != null)
                await Shell.Current.GoToAsync($"//{nameof(MainView)}");
            else
                Message = "Shell niet beschikbaar, navigatie mislukt.";
        }
        catch (Exception ex)
        {
            Message = $"Er is een fout opgetreden: {ex.Message}";
        }
    }
}