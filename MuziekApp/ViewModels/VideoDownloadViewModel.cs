using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuziekApp.Services;

namespace MuziekApp.ViewModels;

public partial class VideoDownloadViewModel : ObservableObject
{
    [ObservableProperty] private string videoTitle;
    [ObservableProperty] private string thumbnail;
    [ObservableProperty] private string downloadStatus = "Klaar om te downloaden";

    private readonly string _videoUrl;
    private readonly DatabaseService _databaseService = new();

    public VideoDownloadViewModel(string title, string thumb, string url)
    {
        VideoTitle = title;
        Thumbnail = thumb;
        _videoUrl = url;
    }

    [RelayCommand]
    private async Task StartDownload()
    {
        DownloadStatus = "Bezig met downloaden...";
        var success = await _databaseService.DownloadFromYouTubeAsync(_videoUrl);
        DownloadStatus = success ? "Download voltooid" : "Download mislukt";

        LocalStorageService.AddDownloadedVideo(new DownloadedVideo
        {
            Title = VideoTitle,
            Thumbnail = Thumbnail,
            Url = _videoUrl,
            Status = DownloadStatus
        });
    }
}