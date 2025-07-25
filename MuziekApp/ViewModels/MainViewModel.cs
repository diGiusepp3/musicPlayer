using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;
using MuziekApp.Models;
using System.Collections.ObjectModel;
using MuziekApp.Views;

namespace MuziekApp.ViewModels;

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
    [ObservableProperty] private bool isShuffleEnabled;
    [ObservableProperty] private double currentSongDuration;
    [ObservableProperty] private double currentSongPosition;

    // search props
    [ObservableProperty] private string searchText;
    [ObservableProperty] private ObservableCollection<SearchResultItem> searchResults = new();
    [ObservableProperty] private bool isSearchResultsVisible;

    // dynamic play/pause
    [ObservableProperty] private string playPauseIcon = "icon_pause.svg";

    public ObservableCollection<SongDto> Songs { get; } = new();
    public ObservableCollection<Artist> Artists { get; } = new();
    public ObservableCollection<Album> Albums { get; } = new();

    public MainViewModel()
    {
        MediaElementService.Current.OnSongChanged += (title, artist) =>
        {
            CurrentSongTitle = title;
            CurrentSongArtist = artist;
            IsMiniPlayerVisible = true;
            PlayPauseIcon = "icon_pause.svg";
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
        await MediaElementService.Current.PlayAsync(song.file_url, song.title, song.artists);
        CurrentSongTitle = song.title;
        CurrentSongArtist = song.artists;
        IsMiniPlayerVisible = true;
        PlayPauseIcon = "icon_pause.svg";
    }

    [RelayCommand]
    private void TogglePlayPause()
    {
        MediaElementService.Current.TogglePlayPause();
        PlayPauseIcon = MediaElementService.Current.IsPlaying ? "icon_pause.svg" : "icon_play.svg";
    }

    [RelayCommand]
    private async Task PlayNext()
    {
        await MediaElementService.Current.PlayNext();
        PlayPauseIcon = "icon_pause.svg";
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

        var results = await _databaseService.SearchYouTubeAsync(SearchText);
        SearchResults.Clear();
        foreach (var r in results) SearchResults.Add(r);
        IsSearchResultsVisible = SearchResults.Any();
    }

    [RelayCommand]
    private async Task OpenVideo(SearchResultItem item)
    {
        if (item == null) return;
        IsSearchResultsVisible = false;
        await Shell.Current.GoToAsync(nameof(VideoDownloadView), new Dictionary<string, object>
        {
            {"title", item.Title},
            {"thumb", item.Thumbnail},
            {"url", item.Url}
        });
    }

    public void UpdatePosition()
    {
        CurrentSongDuration = MediaElementService.Current.Duration.TotalSeconds;
        CurrentSongPosition = MediaElementService.Current.Position.TotalSeconds;
    }
}
