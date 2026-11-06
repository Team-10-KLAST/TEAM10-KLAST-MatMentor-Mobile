using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class TutorPage : ContentPage
{
    public TutorPage(TutorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
