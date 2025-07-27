using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using MuziekApp.Controls;
using MuziekApp.Services;
using MuziekApp.ViewModels;

namespace MuziekApp.Views
{
    public partial class MainView : ContentPage
    {
        readonly IDispatcherTimer _positionTimer;
        readonly IDispatcherTimer _visualizerTimer;
        readonly VisualizerDrawable _visualizer;
        bool _isMiniPlayerExpanded = false;

        MainViewModel ViewModel => BindingContext as MainViewModel;

        public MainView(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            // 1) VisualizerDrawable koppelen
            _visualizer = new VisualizerDrawable();
            VisualizerView.Drawable = _visualizer;

            // 2) Timer voor songpositie
            _positionTimer = Application.Current.Dispatcher.CreateTimer();
            _positionTimer.Interval = TimeSpan.FromMilliseconds(500);
            _positionTimer.Tick += (s, e) => ViewModel?.UpdatePosition();

            // 3) Timer voor visualizer-animatie
            _visualizerTimer = Application.Current.Dispatcher.CreateTimer();
            _visualizerTimer.Interval = TimeSpan.FromMilliseconds(100);
            _visualizerTimer.Tick += (s, e) =>
            {
                _visualizer.Update();
                VisualizerView.Invalidate();
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            _positionTimer.Start();
            _visualizerTimer.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _positionTimer.Stop();
            _visualizerTimer.Stop();
        }

        private void OnSliderDragCompleted(object sender, EventArgs e)
        {
            if (sender is Slider slider)
                MediaElementService.Current.Seek(TimeSpan.FromSeconds(slider.Value));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            LocalStorageService.ClearUser();
            await Shell.Current.GoToAsync(nameof(LoginView));
        }

        private void OnMiniPlayerTapped(object sender, EventArgs e)
        {
            double collapsed = 80;
            double expanded = Height * 0.8;
            double from = MiniPlayerGrid.HeightRequest;
            double to = _isMiniPlayerExpanded ? collapsed : expanded;

            var animation = new Microsoft.Maui.Controls.Animation(
                h => MiniPlayerGrid.HeightRequest = h,
                from, to);

            animation.Commit(this, "ToggleMiniSize", length: 250, easing: Easing.CubicInOut);
            _isMiniPlayerExpanded = !_isMiniPlayerExpanded;
        }
    }
}
