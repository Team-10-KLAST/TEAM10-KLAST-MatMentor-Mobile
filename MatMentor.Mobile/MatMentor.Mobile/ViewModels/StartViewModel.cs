using System.Windows.Input;
using MatMentor.Mobile.Views;

namespace MatMentor.Mobile.ViewModels;

public class StartViewModel
{
    public ICommand PracticeCommand { get; }

    public ICommand TutorCommand { get; }

    public ICommand LogoutCommand { get; }

    public StartViewModel()
    {
        PracticeCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(TopicSelectionPage)));

        TutorCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(TutorPage)));

        LogoutCommand = new Command(
            async () => await LogoutAsync());
    }

    // Returns the user to the welcome page and clears the navigation stack.
    private async Task LogoutAsync()
    {
        await Shell.Current.GoToAsync("//WelcomePage");
    }
}