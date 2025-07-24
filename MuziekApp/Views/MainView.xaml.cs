using MuziekApp.Services;
using MuziekApp.ViewModels;

namespace MuziekApp.Views
{
    public partial class MainView : ContentPage
    {
        private readonly IDispatcherTimer _timer = Application.Current.Dispatcher.CreateTimer();

        public MainView(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += (s, e) =>
            {
                ProgressSlider.Maximum = MediaElementService.Current.Duration.TotalSeconds;
                ProgressSlider.Value = MediaElementService.Current.Position.TotalSeconds;
            };
            _timer.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer.Stop();
        }

        private void OnSliderDragCompleted(object sender, EventArgs e)
        {
            if (sender is Slider slider)
            {
                MediaElementService.Current.Seek(TimeSpan.FromSeconds(slider.Value));
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            LocalStorageService.ClearUser();
            await Shell.Current.GoToAsync(nameof(LoginView));
        }
    }
}