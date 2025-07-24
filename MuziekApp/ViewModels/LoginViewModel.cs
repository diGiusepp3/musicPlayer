using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using MuziekApp.Views;

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
        if (success)
        {
            Message = "Welkom!";
            await Shell.Current.GoToAsync(nameof(MainView)); // route uit AppShell gebruiken
        }
        else
        {
            Message = "Verkeerde gegevens!";
        }
    }
}