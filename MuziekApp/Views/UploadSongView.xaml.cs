using MuziekApp.ViewModels;

namespace MuziekApp.Views;

public partial class UploadSongView : ContentPage
{
    public UploadSongView(UploadSongViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
