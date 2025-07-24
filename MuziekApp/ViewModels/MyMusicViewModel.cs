using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MuziekApp.ViewModels
{
    public partial class MyMusicViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://music.datadrive.be/api/") };

        [ObservableProperty] private string message;
        public ObservableCollection<SongItem> Songs { get; } = new();

        public MyMusicViewModel()
        {
            LoadSongsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadSongs()
        {
            Songs.Clear();
            var user = LocalStorageService.LoadUser();
            if (user == null)
            {
                Message = "Geen gebruiker gevonden";
                return;
            }

            var response = await _httpClient.GetFromJsonAsync<MyMusicResponse>($"users/get_songs.php?user_id={user.Id}");
            if (response?.Status == "ok")
            {
                foreach (var song in response.Songs)
                    Songs.Add(song);
            }
            else
            {
                Message = "Kon geen muziek ophalen";
            }
        }

        public class SongItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Artists { get; set; }
            public string File_Url { get; set; }
        }

        private class MyMusicResponse
        {
            public string Status { get; set; }
            public List<SongItem> Songs { get; set; }
        }
    }
}