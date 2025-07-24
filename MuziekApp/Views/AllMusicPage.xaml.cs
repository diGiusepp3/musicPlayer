using MuziekApp.ViewModels;

namespace MuziekApp.Views
{
    public partial class AllMusicPage : ContentPage
    {
        private readonly AllMusicViewModel _vm;

        public AllMusicPage(AllMusicViewModel vm)   // vm wordt via DI meegegeven
        {
            InitializeComponent();
            BindingContext = vm;
            _vm = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadDataAsync();
        }
    }
}