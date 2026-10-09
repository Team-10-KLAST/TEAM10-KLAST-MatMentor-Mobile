using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class TutorPage : ContentPage
{
    public TutorPage()
    {
        InitializeComponent();
        BindingContext = new TutorViewModel();
    }
}