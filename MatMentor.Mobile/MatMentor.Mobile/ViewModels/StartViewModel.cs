using System.Windows.Input;
using MatMentor.Mobile.Views;

namespace MatMentor.Mobile.ViewModels;

public class StartViewModel
{
    public ICommand PracticeCommand { get; }

    public ICommand TutorCommand { get; }

    public StartViewModel()
    {
        PracticeCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(TopicSelectionPage)));

        TutorCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(TutorPage)));
    }
}