using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Models;
using MuziekApp.Services;
using System.Collections.ObjectModel;

namespace MuziekApp.ViewModels
{
    public partial class AllMusicViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Song> songs = new();

        [ObservableProperty]
        private ObservableCollection<Artist> artists = new();

        [ObservableProperty]
        private ObservableCollection<Album> albums = new();

        [ObservableProperty]
        private bool isLoading;

        public AllMusicViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                var allSongs = await _databaseService.GetAllSongsAsync();
                var allArtists = await _databaseService.GetAllArtistsAsync();
                var allAlbums = await _databaseService.GetAllAlbumsAsync();

                Songs.Clear();
                foreach (var song in allSongs) Songs.Add(song);

                Artists.Clear();
                foreach (var artist in allArtists) Artists.Add(artist);

                Albums.Clear();
                foreach (var album in allAlbums) Albums.Add(album);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}