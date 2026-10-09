using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class CreateStudentProfilePage : ContentPage
{
    public CreateStudentProfilePage()
    {
        InitializeComponent();
        BindingContext = new CreateStudentProfileViewModel();
    }
}