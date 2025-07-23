using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;

namespace MuziekApp.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly DatabaseService _database;

    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string message;

    public RegisterViewModel(DatabaseService database)
    {
        _database = database;
    }

    [RelayCommand]
    private async Task Register()
    {
        var success = await _database.RegisterUserAsync(Email, Password);
        Message = success ? "Registratie gelukt!" : "Gebruiker bestaat al!";
    }
}