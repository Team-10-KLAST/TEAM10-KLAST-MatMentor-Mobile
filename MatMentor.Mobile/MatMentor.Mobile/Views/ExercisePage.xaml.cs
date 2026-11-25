// File: MatMentor.Mobile/Views/ExercisePage.xaml.cs
using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class ExercisePage : ContentPage
{
    public ExercisePage(ExerciseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Handles Android back as an explicit exit, never as navigation to earlier exercises.
    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is ExerciseViewModel viewModel &&
            viewModel.CanLeave &&
            viewModel.LeaveExerciseCommand.CanExecute(null))
        {
            viewModel.LeaveExerciseCommand.Execute(null);
            return true;
        }

        return true;
    }
}