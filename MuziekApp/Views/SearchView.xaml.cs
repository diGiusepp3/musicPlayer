using MuziekApp.Services;
using MuziekApp.ViewModels;

namespace MuziekApp.Views
{
    public partial class SearchView : ContentPage
    {
        private readonly SearchViewModel _vm;

        public SearchView(SearchViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _vm = vm;
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[SEARCH] Text changed: {e.NewTextValue}");

            if (!string.IsNullOrWhiteSpace(e.NewTextValue) && e.NewTextValue.Length >= 2)
            {
                await _vm.PerformSearchAsync(e.NewTextValue);
                ResultsDropdown.IsVisible = _vm.Results.Count > 0;
            }
            else
            {
                _vm.Results.Clear();
                ResultsDropdown.IsVisible = false;
            }
        }

        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is SearchResult result)
            {
                DisplayAlert("Toegevoegd", $"{result.Title} is toegevoegd.", "OK");
            }
        }
    }
}