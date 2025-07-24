using System.Net.Http.Json;

namespace MuziekApp;

public class StartupCheckService
{
    private readonly HttpClient _httpClient;

    public StartupCheckService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://music.datadrive.be/api/")
        };
    }

    public async Task<bool> RunCheckAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<DbCheckResponse>("dbcheck.php");
            if (response is not null && response.Status == "ok")
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

    private class DbCheckResponse
    {
        public string Status { get; set; }
    }
}