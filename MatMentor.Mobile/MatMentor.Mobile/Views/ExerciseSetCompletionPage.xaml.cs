using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class ExerciseSetCompletionPage : ContentPage
{
    public ExerciseSetCompletionPage()
    {
        InitializeComponent();
        BindingContext = new ExerciseSetCompletionViewModel();
    }
}