
using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class StartPage : ContentPage
{
    public StartPage(StartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
