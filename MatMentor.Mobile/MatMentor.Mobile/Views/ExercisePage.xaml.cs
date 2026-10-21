using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class ExercisePage : ContentPage
{
    public ExercisePage()
    {
        InitializeComponent();
        BindingContext = new ExerciseViewModel();
    }

    // Uses the previous-exercise behavior when the Android back button is pressed.
    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is ExerciseViewModel viewModel &&
            viewModel.PreviousExerciseCommand.CanExecute(null))
        {
            viewModel.PreviousExerciseCommand.Execute(null);
            return true;
        }

        return base.OnBackButtonPressed();
    }
}