using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using MuziekApp.Views;

namespace MuziekApp.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly DatabaseService _database;

    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string confirmPassword;
    [ObservableProperty] private string message;

    public RegisterViewModel(DatabaseService database)
    {
        _database = database;
    }

    [RelayCommand]
    private async Task Register()
    {
        Message = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            Message = "Vul alle velden in.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            Message = "Wachtwoorden komen niet overeen.";
            return;
        }

        var success = await _database.RegisterUserAsync(Email, Password);
        if (!success)
        {
            Message = "Gebruiker bestaat al!";
            return;
        }

        Message = "Registratie gelukt! Je wordt doorgestuurd...";
        await Shell.Current.GoToAsync(nameof(LoginView));
    }
}