using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
        BindingContext = new StartViewModel();
    }
}