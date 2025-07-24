using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MuziekApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<MusicItem> recommendedMusic;
        [ObservableProperty] private ObservableCollection<ArtistItem> popularArtists;
        [ObservableProperty] private string welcomeMessage;
        [ObservableProperty] private bool showUploadButton;

        public MainViewModel()
        {
            var userId = Preferences.Get("user_id", 0);
            var displayName = Preferences.Get("display_name", "User");

            WelcomeMessage = $"Goedemiddag, {displayName}";
            ShowUploadButton = (userId == 2 || userId == 3 || userId == 4);
            
            RecommendedMusic = new ObservableCollection<MusicItem>
            {
                new MusicItem { Title="Summer Vibes", Artist="DJ X", Image="sample1.jpg" },
                new MusicItem { Title="Chill Beats", Artist="LoFi Crew", Image="sample2.jpg" },
                new MusicItem { Title="Workout Mix", Artist="Fit Beats", Image="sample3.jpg" }
            };

            PopularArtists = new ObservableCollection<ArtistItem>
            {
                new ArtistItem { Name="Artist 1", Image="artist1.png" },
                new ArtistItem { Name="Artist 2", Image="Artist2.png" },
                new ArtistItem { Name="Artist 3", Image="Artist3.png" }
            };
        }
    }

    public class MusicItem
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Image { get; set; }
    }

    public class ArtistItem
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }
}