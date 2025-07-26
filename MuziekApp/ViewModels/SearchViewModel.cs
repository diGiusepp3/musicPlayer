using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using MuziekApp.Models;

namespace MuziekApp.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly HttpClient _http = new();

        [ObservableProperty] private string searchQuery;
        public ObservableCollection<SearchResultItem> Results { get; } = new();

        [RelayCommand]
        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery)) return;

            Results.Clear();
            try
            {
                var url = $"https://music.datadrive.be/api/functions/search.php?q={Uri.EscapeDataString(SearchQuery)}";
                var items = await _http.GetFromJsonAsync<List<SearchResultItem>>(url);

                if (items != null)
                {
                    foreach (var item in items)
                        Results.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Zoekfout: " + ex.Message);
            }
        }

        [RelayCommand]
        private async Task DownloadMp3(SearchResultItem item)
        {
            if (item == null) return;

            try
            {
                // Roep jouw bestaande download endpoint aan
                var response = await _http.PostAsJsonAsync(
                    "https://music.datadrive.be/functions/download.php?single=1",
                    new { url = item.Url, type = "audio" });

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine($"Download mislukt: {response.StatusCode}");
                else
                    Console.WriteLine($"Download gestart: {item.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Downloadfout: " + ex.Message);
            }
        }
    }
}