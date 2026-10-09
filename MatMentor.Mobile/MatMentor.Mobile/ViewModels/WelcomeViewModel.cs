using System.Windows.Input;
using MatMentor.Mobile.Views;

namespace MatMentor.Mobile.ViewModels;

public class WelcomeViewModel
{
    public ICommand LoginCommand { get; }

    public ICommand CreateProfileCommand { get; }

    public WelcomeViewModel()
    {
        LoginCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(LoginPage)));

        CreateProfileCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(CreateStudentProfilePage)));
    }
}