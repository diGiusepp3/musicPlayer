using CommunityToolkit.Mvvm.ComponentModel;
using MuziekApp.Services;
using System.Collections.ObjectModel;

namespace MuziekApp.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly SearchService _searchService;

        [ObservableProperty]
        private ObservableCollection<SearchResult> results = new();

        public SearchViewModel(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task PerformSearchAsync(string value)
        {
            System.Diagnostics.Debug.WriteLine($"[SEARCH] Performing search for: {value}");
            var found = await _searchService.SearchAsync(value);
            Results.Clear();
            foreach (var item in found)
            {
                Results.Add(item);
                System.Diagnostics.Debug.WriteLine($"[SEARCH] Found: {item.Title}");
            }
        }
    }
}