using MuziekApp.ViewModels;

namespace MuziekApp.Views
{
    public partial class LoginView : ContentPage
    {
        public LoginView(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}