using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MatMentor.Mobile.Views;
using System.Collections.Generic;

namespace MatMentor.Mobile.ViewModels;

public class TopicSelectionViewModel : INotifyPropertyChanged
{
    private string _selectedTopicName = string.Empty;
    private string _selectionMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string SelectedTopicName
    {
        get => _selectedTopicName;
        private set
        {
            _selectedTopicName = value;
            OnPropertyChanged();
        }
    }

    public string SelectionMessage
    {
        get => _selectionMessage;
        private set
        {
            _selectionMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand OpenTutorCommand { get; }

    public ICommand SelectTopicCommand { get; }

    public ICommand LogoutCommand { get; }

    public TopicSelectionViewModel()
    {
        OpenTutorCommand = new Command(
            async () => await Shell.Current.GoToAsync(nameof(TutorPage)));

        SelectTopicCommand = new Command<string>(SelectTopic);

        LogoutCommand = new Command(
            async () => await LogoutAsync());
    }

    // Registers the selected topic and continues to the exercise set.
    private async void SelectTopic(string? topicName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return;
        }

        SelectedTopicName = topicName;
        SelectionMessage = $"Du har valgt: {topicName}";

        await Shell.Current.GoToAsync(
            nameof(ExerciseSetPage),
            new Dictionary<string, object>
            {
                ["topicName"] = topicName
            });
    }

    // Returns the user to the welcome page and clears the navigation stack.
    private async Task LogoutAsync()
    {
        await Shell.Current.GoToAsync("//WelcomePage");
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}