using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using MuziekApp.Controls;
using MuziekApp.Services;
using MuziekApp.ViewModels;

namespace MuziekApp.Views;

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
    
    // SearchBar functies
    private CancellationTokenSource _searchCts;

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(300, _searchCts.Token); // debounce
            if (BindingContext is MainViewModel vm)
            {
                await vm.PerformSearchAsync(e.NewTextValue);
                ResultsDropdown.IsVisible = vm.Results.Count > 0;
            }
        }
        catch (TaskCanceledException) { /* genegeerd */ }
    }


    private async void OnAddButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is SearchResult result)
        {
            try
            {
                // URL van add.php (zelfde map als search.php)
                var url = $"https://music.datadrive.be/api/functions/add.php";

                // JSON body met videoId en titel (aanpassen naar wat add.php verwacht)
                var payload = new
                {
                    videoId = result.VideoId,
                    title = result.Title,
                    channel = result.Channel,
                    thumbnail = result.Thumbnail
                };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ADD] Response: {resp}");
                    await DisplayAlert("Succes", $"{result.Title} toegevoegd en download gestart.", "OK");
                }
                else
                {
                    Console.WriteLine($"[ADD] Error: {response.StatusCode}");
                    await DisplayAlert("Fout", $"Kon {result.Title} niet toevoegen.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ADD] Exception: " + ex.Message);
                await DisplayAlert("Fout", "Er ging iets mis bij het toevoegen.", "OK");
            }
        }
    }


    
    // Miniplayer functies
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