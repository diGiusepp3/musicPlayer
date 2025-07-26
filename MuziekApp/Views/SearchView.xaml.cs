using MuziekApp.ViewModels;

namespace MuziekApp.Views
{
    public partial class SearchView : ContentPage
    {
        private SearchViewModel ViewModel => BindingContext as SearchViewModel;

        public SearchView(SearchViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Eventueel: automatisch focus op zoekbalk of laatste zoekopdracht uitvoeren
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Eventueel: opschonen of timers stoppen (nu niet nodig)
        }
    }
}