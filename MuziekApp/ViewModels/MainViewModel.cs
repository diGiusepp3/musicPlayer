using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Models;
using MuziekApp.Services;
using MuziekApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui;

// Dit ViewModel gebruikt voortaan altijd de playlist uit MediaElementService.
// Bij autoplay (OnSongEnded) wacht het 200 ms voordat het PlayNext aanroept om native crashes te voorkomen.

namespace MuziekApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SongService _songService = new();
        private readonly DatabaseService _databaseService = new();
        private readonly HttpClient _http = new();

        private int currentIndex = -1;

        [ObservableProperty] private string message;
        [ObservableProperty] private string welcomeMessage = "Welkom terug!";
        [ObservableProperty] private bool showUploadButton = true;
        [ObservableProperty] private string currentSongTitle;
        [ObservableProperty] private string currentSongArtist;
        [ObservableProperty] private bool isMiniPlayerVisible;
        [ObservableProperty] private bool isShuffleEnabled;
        [ObservableProperty] private double currentSongDuration;
        [ObservableProperty] private double currentSongPosition;

        // search props
        [ObservableProperty] private string searchText;
        [ObservableProperty] private ObservableCollection<SearchResultItem> searchResults = new();
        [ObservableProperty] private bool isSearchResultsVisible;

        // dynamic play/pause (gebruik PNG iconen voor betrouwbaarheid)
        [ObservableProperty] private string playPauseIcon = "icon_pause.png";

        public ObservableCollection<SongDto> Songs { get; } = new();
        public ObservableCollection<Artist> Artists { get; } = new();
        public ObservableCollection<Album> Albums { get; } = new();

        public MainViewModel()
        {
            // Timer om positie slider bij te werken
            Microsoft.Maui.Controls.Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                UpdatePosition();
                return true;
            });

            // Autoplay: als een nummer eindigt, wacht 200 ms en speel het volgende
            MediaElementService.Current.OnSongEnded += async () =>
            {
                Console.WriteLine("[PLAYER] Song ended...");
                await Task.Delay(200);
                try
                {
                    await MainThread.InvokeOnMainThreadAsync(PlayNext);
                    Console.WriteLine("[PLAYER] Autoplaying next song...");
                }
                catch (Exception ex)
                {
                    Message = "Autoplay error: " + ex;
                }
            };

            // SongChanged update UI
            MediaElementService.Current.OnSongChanged += (title, artist) =>
            {
                CurrentSongTitle = title;
                CurrentSongArtist = artist;
                IsMiniPlayerVisible = true;
                PlayPauseIcon = "icon_pause.png";
            };

            // Laad alle data én stel playlist in
            LoadAllDataCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadAllData()
        {
            try
            {
                Songs.Clear();
                var songResult = await _songService.GetAllSongsAsync();
                foreach (var song in songResult)
                {
                    if (!string.IsNullOrWhiteSpace(song.file_url) && !song.file_url.StartsWith("http"))
                        song.file_url = "https://music.datadrive.be" + song.file_url;

                    if (!string.IsNullOrWhiteSpace(song.cover_url) && !song.cover_url.StartsWith("http"))
                        song.cover_url = "https://music.datadrive.be" + song.cover_url;

                    Songs.Add(song);
                }

                // Stel playlist in bij MediaElementService
                MediaElementService.Current.SetPlaylist(
                    Songs.Select(s => new PlaylistSong(s.title, s.artists, s.file_url))
                );

                Artists.Clear();
                foreach (var artist in await _databaseService.GetAllArtistsAsync())
                {
                    if (!string.IsNullOrWhiteSpace(artist.ImageUrl) && !artist.ImageUrl.StartsWith("http"))
                        artist.ImageUrl = "https://music.datadrive.be" + artist.ImageUrl;
                    Artists.Add(artist);
                }

                Albums.Clear();
                foreach (var album in await _databaseService.GetAllAlbumsAsync())
                {
                    if (!string.IsNullOrWhiteSpace(album.CoverUrl) && !album.CoverUrl.StartsWith("http"))
                        album.CoverUrl = "https://music.datadrive.be" + album.CoverUrl;
                    Albums.Add(album);
                }
            }
            catch (Exception ex)
            {
                Message = "Kon gegevens niet laden: " + ex.Message;
            }
        }

        [RelayCommand]
        private async Task PlaySong(SongDto song)
        {
            if (song == null) return;

            currentIndex = Songs.IndexOf(song);
            await MediaElementService.Current.PlayAsync(song.file_url, song.title, song.artists);
            // UI wordt via OnSongChanged geüpdatet
        }

        [RelayCommand]
        private void TogglePlayPause()
        {
            MediaElementService.Current.TogglePlayPause();
            PlayPauseIcon = MediaElementService.Current.IsPlaying ? "icon_pause.png" : "icon_play.png";
        }

        [RelayCommand]
        private async Task PlayNext()
        {
            if (Songs.Count == 0)
            {
                Message = "Geen nummers beschikbaar";
                return;
            }

            if (currentIndex < 0)
                currentIndex = 0;
            else
            {
                currentIndex = IsShuffleEnabled
                    ? new Random().Next(Songs.Count)
                    : (currentIndex + 1) % Songs.Count;
            }

            var next = Songs[currentIndex];
            if (string.IsNullOrWhiteSpace(next.file_url))
            {
                Message = "Volgend nummer is ongeldig";
                return;
            }

            await MediaElementService.Current.PlayAsync(next.file_url, next.title, next.artists);
        }

        [RelayCommand]
        private void ToggleShuffle() => IsShuffleEnabled = !IsShuffleEnabled;

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SearchResults.Clear();
                IsSearchResultsVisible = false;
                return;
            }

            try
            {
                var url = $"https://music.datadrive.be/api/functions/search.php?q={Uri.EscapeDataString(SearchText)}";
                var items = await _http.GetFromJsonAsync<List<SearchResultItem>>(url);

                SearchResults.Clear();
                if (items != null)
                    foreach (var it in items)
                        SearchResults.Add(it);

                IsSearchResultsVisible = SearchResults.Any();
            }
            catch (Exception ex)
            {
                Message = "Zoeken mislukt: " + ex.Message;
            }
        }

        [RelayCommand]
        private async Task OpenVideo(SearchResultItem item)
        {
            if (item == null) return;

            IsSearchResultsVisible = false;
            var url = string.IsNullOrEmpty(item.Url)
                ? $"https://www.youtube.com/watch?v={item.VideoId}"
                : item.Url;

            /* Todo:
             await Shell.Current.GoToAsync(nameof(VideoDownloadView), new Dictionary<string, object>
                {
                    ["title"] = item.Title,
                    ["thumb"] = item.Thumbnail,
                    ["url"]   = url
                });
            */
        }

        public void UpdatePosition()
        {
            CurrentSongDuration = MediaElementService.Current.Duration.TotalSeconds;
            CurrentSongPosition = MediaElementService.Current.Position.TotalSeconds;
        }
    }
}