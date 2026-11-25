// File: MatMentor.Mobile/Views/ExerciseSetPage.xaml.cs
using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class ExerciseSetPage : ContentPage
{
    public ExerciseSetPage(ExerciseSetViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}