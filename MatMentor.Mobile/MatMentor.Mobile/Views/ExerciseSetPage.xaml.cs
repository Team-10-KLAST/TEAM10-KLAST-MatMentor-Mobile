using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class ExerciseSetPage : ContentPage
{
    public ExerciseSetPage()
    {
        InitializeComponent();
        BindingContext = new ExerciseSetViewModel();
    }
}