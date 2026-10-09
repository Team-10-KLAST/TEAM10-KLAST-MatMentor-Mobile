using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
        BindingContext = new WelcomeViewModel();
    }
}