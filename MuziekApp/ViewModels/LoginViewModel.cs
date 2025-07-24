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
        var user = await _database.LoginAndGetUserAsync(Email, Password);
        if (user == null)
        {
            Message = "Verkeerde gegevens!";
            return;
        }

        Preferences.Set("user_id", user.Id);
        Preferences.Set("user_email", user.Email);
        Preferences.Set("is_premium", user.IsPremium);

        Message = $"Welkom {user.Email}!";
        await Shell.Current.GoToAsync($"{nameof(MainView)}");
    }

}