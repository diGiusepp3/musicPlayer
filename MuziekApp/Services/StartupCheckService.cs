using MuziekApp.Services;

namespace MuziekApp;

public class StartupCheckService
{
    private readonly DatabaseService _databaseService;

    public StartupCheckService()
    {
        _databaseService = new DatabaseService();
    }

    /// <summary>
    /// Voert de startcheck uit: internet + API + database.
    /// </summary>
    public async Task<bool> RunCheckAsync()
    {
        try
        {
            bool dbOk = await _databaseService.CheckConnectionAsync();

            if (dbOk)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.DisplayAlert("Status", "Success (API + DB ok)", "OK"));
                return true;
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.DisplayAlert("Status", "No success (API of DB error)", "OK"));
                return false;
            }
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                Application.Current.MainPage.DisplayAlert("Fout", $"API fout: {ex.Message}", "OK"));
            return false;
        }
    }
}