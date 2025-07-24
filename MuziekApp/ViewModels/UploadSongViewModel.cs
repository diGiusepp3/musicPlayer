using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace MuziekApp.ViewModels
{
    public partial class UploadSongViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://music.datadrive.be/api/") };

        [ObservableProperty] private int selectedAlbumId;
        [ObservableProperty] private string title;
        [ObservableProperty] private int duration;
        [ObservableProperty] private int trackNumber;
        [ObservableProperty] private string audioUrl;
        [ObservableProperty] private string filePath;
        [ObservableProperty] private string message;

        public ObservableCollection<AlbumItem> Albums { get; } = new();

        public UploadSongViewModel()
        {
            LoadAlbumsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadAlbums()
        {
            try
            {
                var albums = await _httpClient.GetFromJsonAsync<List<AlbumItem>>("albums/get_all.php");
                Albums.Clear();
                if (albums != null)
                    foreach (var album in albums)
                        Albums.Add(album);
            }
            catch
            {
                Message = "Kon albums niet laden.";
            }
        }

        [RelayCommand]
        private async Task UploadSong()
        {
            try
            {
                var payload = new
                {
                    album_id = SelectedAlbumId,
                    title = Title,
                    duration = Duration,
                    track_number = TrackNumber,
                    audio_url = AudioUrl,
                    file_path = FilePath
                };

                var response = await _httpClient.PostAsJsonAsync("songs/add_song.php", payload);
                var raw = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<ApiResponse>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Message = json?.Status == "ok" ? "Song toegevoegd!" : "Fout bij toevoegen!";
            }
            catch (Exception ex)
            {
                Message = "Fout: " + ex.Message;
            }
        }

        public class AlbumItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }

        private class ApiResponse
        {
            public string Status { get; set; }
        }
        
        [RelayCommand]
        private async Task AddNewAlbum()
        {
            // Simpele dialoog voor nu, later kan dit een volledige view zijn
            string result = await Shell.Current.DisplayPromptAsync("Nieuw Album", "Naam van het album:");
            if (!string.IsNullOrWhiteSpace(result))
            {
                var payload = new { title = result };
                var response = await _httpClient.PostAsJsonAsync("albums/add_album.php", payload);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAlbums(); // lijst opnieuw laden
                    Message = "Album toegevoegd!";
                }
                else
                {
                    Message = "Fout bij toevoegen album.";
                }
            }
        }
    }
}
