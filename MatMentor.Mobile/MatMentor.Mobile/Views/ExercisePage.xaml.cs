using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class ExercisePage : ContentPage
{
    public ExercisePage()
    {
        InitializeComponent();
        BindingContext = new ExerciseViewModel();
    }
}