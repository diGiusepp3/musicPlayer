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

namespace MuziekApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SongService _songService = new();
        private readonly DatabaseService _databaseService = new();
        private readonly HttpClient _http = new();
        private readonly SearchService _searchService;

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

        // Live search
        public ObservableCollection<SearchResult> Results { get; set; } = new();
    
        // dynamic play/pause
        [ObservableProperty] private string playPauseIcon = "icon_pause.png";

        public ObservableCollection<SongDto> Songs { get; } = new();
        public ObservableCollection<Artist> Artists { get; } = new();
        public ObservableCollection<Album> Albums { get; } = new();

        public MainViewModel()
        {
            _searchService = new SearchService(); // <-- FIX

            // Timer voor positie slider
            Microsoft.Maui.Controls.Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                UpdatePosition();
                return true;
            });

            // Autoplay event
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

            // SongChanged event
            MediaElementService.Current.OnSongChanged += (title, artist) =>
            {
                CurrentSongTitle = title;
                CurrentSongArtist = artist;
                IsMiniPlayerVisible = true;
                PlayPauseIcon = "icon_pause.png";
            };

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

        public async Task PerformSearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Results.Clear();
                return;
            }

            try
            {
                Console.WriteLine($"[SEARCH] API call met query: {query}");
                var found = await _searchService.SearchAsync(query);

                Results.Clear();
                foreach (var item in found)
                {
                    Console.WriteLine($"[SEARCH] Resultaat: {item.Title} | {item.Channel} | {item.Thumbnail}");
                    Results.Add(item);
                }

                Console.WriteLine($"[SEARCH] {Results.Count} resultaten gevonden.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SEARCH] Fout: " + ex.Message);
            }
        }


        public void UpdatePosition()
        {
            CurrentSongDuration = MediaElementService.Current.Duration.TotalSeconds;
            CurrentSongPosition = MediaElementService.Current.Position.TotalSeconds;
        }
    }
}
