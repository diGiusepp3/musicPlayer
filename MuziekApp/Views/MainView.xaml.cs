using MuziekApp.Services;
using MuziekApp.ViewModels;

namespace MuziekApp.Views
{
    public partial class MainView : ContentPage
    {
        public MainView(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            LocalStorageService.ClearUser();
            await Shell.Current.GoToAsync(nameof(LoginView));
        }

        private async void OnUploadSongClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(UploadSongView));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
        }
    }
}