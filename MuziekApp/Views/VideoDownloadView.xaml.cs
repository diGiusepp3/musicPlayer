using MuziekApp.ViewModels;

namespace MuziekApp.Views;

public partial class VideoDownloadView : ContentPage
{
    public VideoDownloadView(VideoDownloadViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}