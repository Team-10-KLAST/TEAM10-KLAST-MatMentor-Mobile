
using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class CreateStudentProfilePage : ContentPage
{
    public CreateStudentProfilePage(
        CreateStudentProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
