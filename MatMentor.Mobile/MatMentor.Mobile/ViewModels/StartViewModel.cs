
using System.Windows.Input;
using MatMentor.Mobile.Services;
using MatMentor.Mobile.Views;

namespace MatMentor.Mobile.ViewModels;

public class StartViewModel
{
    private readonly AuthSessionService _authSessionService;

    public ICommand PracticeCommand { get; }

    public ICommand TutorCommand { get; }

    public ICommand LogoutCommand { get; }

    public StartViewModel(AuthSessionService authSessionService)
    {
        _authSessionService = authSessionService;

        PracticeCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(TopicSelectionPage)));

        TutorCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(TutorPage)));

        LogoutCommand = new Command(
            async () => await LogoutAsync());
    }

    // Removes the stored authentication token and returns to the welcome page.
    private async Task LogoutAsync()
    {
        _authSessionService.ClearToken();

        await Shell.Current.GoToAsync("//WelcomePage");
    }
}
