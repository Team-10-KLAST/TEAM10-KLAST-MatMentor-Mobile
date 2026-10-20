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

    public ICommand SelectTopicCommand { get; }

    public TopicSelectionViewModel()
    {
        SelectTopicCommand = new Command<string>(SelectTopic);
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

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}