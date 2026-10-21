using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class ExerciseSetCompletionPage : ContentPage
{
    public ExerciseSetCompletionPage()
    {
        InitializeComponent();
        BindingContext = new ExerciseSetCompletionViewModel();
    }

    // Prevents returning to an exercise after the exercise set has been completed.
    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}