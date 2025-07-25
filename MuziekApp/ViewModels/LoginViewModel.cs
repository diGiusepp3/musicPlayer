using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using MuziekApp.Views;
using MuziekApp.Models;   // <- toegevoegd zodat User herkend wordt

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

        // Sla gebruiker lokaal op via REST
        LocalStorageService.SaveUser(user);

        Message = $"Welkom {user.DisplayName}!";
        await Shell.Current.GoToAsync($"//{nameof(MainView)}");
    }
}