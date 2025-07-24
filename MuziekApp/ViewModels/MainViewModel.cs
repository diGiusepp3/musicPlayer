using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using System.Collections.ObjectModel;

namespace MuziekApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SongService _songService = new();

        [ObservableProperty] private string message;
        [ObservableProperty] private string welcomeMessage = "Welkom terug!";
        [ObservableProperty] private bool showUploadButton = true;
        [ObservableProperty] private string currentSongTitle;
        [ObservableProperty] private string currentSongArtist;
        [ObservableProperty] private bool isMiniPlayerVisible;

        public ObservableCollection<Song> Songs { get; } = new();

        public MainViewModel()
        {
            LoadSongsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadSongs()
        {
            try
            {
                Songs.Clear();
                var result = await _songService.GetAllSongsAsync();
                foreach (var song in result)
                {
                    // API levert file_url, we maken hier absolute URL van
                    if (!song.file_url.StartsWith("http"))
                        song.file_url = "https://music.datadrive.be" + song.file_url;

                    Songs.Add(song);
                }
            }
            catch (Exception ex)
            {
                Message = "Kon songs niet laden: " + ex.Message;
            }
        }

        [RelayCommand]
        private async Task PlaySong(Song song)
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