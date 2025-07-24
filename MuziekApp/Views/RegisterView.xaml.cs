using MuziekApp.ViewModels;

namespace MuziekApp.Views
{
    public partial class RegisterView : ContentPage
    {
        public RegisterView(RegisterViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}