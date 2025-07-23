using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;

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
        var success = await _database.LoginUserAsync(Email, Password);
        Message = success ? "Welkom!" : "Verkeerde gegevens!";
    }
}