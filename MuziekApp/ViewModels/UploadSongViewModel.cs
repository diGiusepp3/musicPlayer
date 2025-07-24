using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using MuziekApp.Services;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MuziekApp.ViewModels
{
    public partial class UploadSongViewModel : ObservableObject
    {
        [ObservableProperty] private string title;
        [ObservableProperty] private string selectedFilePath;
        [ObservableProperty] private string message;

        [RelayCommand]
        private async Task PickFile()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "audio/mpeg" } },    // Android MIME type
                { DevicePlatform.iOS, new[] { "public.mp3" } },        // iOS UTI
                { DevicePlatform.WinUI, new[] { ".mp3" } },            // Windows
                { DevicePlatform.MacCatalyst, new[] { "public.mp3" } },// Mac
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecteer een MP3 bestand",
                FileTypes = customFileType
            });

            if (result != null)
            {
                SelectedFilePath = result.FullPath;
            }
        }

        [RelayCommand]
        private async Task Upload()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(SelectedFilePath))
            {
                Message = "Titel en bestand zijn verplicht.";
                return;
            }

            try
            {
                using var httpClient = new HttpClient();
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(Title), "title");
                content.Add(new StringContent("[1]"), "artists"); // voorbeeld: 1 = artist_id
                content.Add(new StreamContent(File.OpenRead(SelectedFilePath))
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("audio/mpeg")
                    }
                }, "file", Path.GetFileName(SelectedFilePath));

                var response = await httpClient.PostAsync("https://music.datadrive.be/api/songs/add_song.php", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Message = response.IsSuccessStatusCode ? "Upload gelukt!" : $"Fout: {responseBody}";
            }
            catch (Exception ex)
            {
                Message = $"Upload mislukt: {ex.Message}";
            }
        }
    }
}
