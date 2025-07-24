using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using MuziekApp.Models;
using System.Collections.ObjectModel;

namespace MuziekApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SongService _songService = new();
        private readonly DatabaseService _databaseService = new();

        [ObservableProperty] private string message;
        [ObservableProperty] private string welcomeMessage = "Welkom terug!";
        [ObservableProperty] private bool showUploadButton = true;
        [ObservableProperty] private string currentSongTitle;
        [ObservableProperty] private string currentSongArtist;
        [ObservableProperty] private bool isMiniPlayerVisible;

        public ObservableCollection<SongDto> Songs { get; } = new();
        public ObservableCollection<Artist> Artists { get; } = new();
        public ObservableCollection<Album> Albums { get; } = new();

        public MainViewModel()
        {
            LoadAllDataCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadAllData()
        {
            try
            {
                // === SONGS ===
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

                // === ARTISTS ===
                Artists.Clear();
                var artistResult = await _databaseService.GetAllArtistsAsync();
                foreach (var artist in artistResult)
                {
                    if (!string.IsNullOrWhiteSpace(artist.ImageUrl) && !artist.ImageUrl.StartsWith("http"))
                        artist.ImageUrl = "https://music.datadrive.be" + artist.ImageUrl;

                    Artists.Add(artist);
                }

                // === ALBUMS ===
                Albums.Clear();
                var albumResult = await _databaseService.GetAllAlbumsAsync();
                foreach (var album in albumResult)
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

            await MediaElementService.Current.PlayAsync(song.file_url, song.title, song.artists);
            CurrentSongTitle = song.title;
            CurrentSongArtist = song.artists;
            IsMiniPlayerVisible = true;
        }

        [RelayCommand]
        private void TogglePlayPause()
        {
            Console.WriteLine("[MiniPlayer] TogglePlayPause triggered");
            MediaElementService.Current.TogglePlayPause();
        }

        [RelayCommand]
        private void StopSong()
        {
            Console.WriteLine("[MiniPlayer] StopSong triggered");
            MediaElementService.Current.Stop();
            IsMiniPlayerVisible = false;
        }
    }
}
