using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MatMentor.Mobile.Views;
using System.Collections.Generic;

namespace MatMentor.Mobile.ViewModels;

public class ExerciseSetViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private string _topicName = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string TopicName
    {
        get => _topicName;
        private set
        {
            _topicName = value;
            OnPropertyChanged();
        }
    }

    public ICommand StartExerciseSetCommand { get; }

    public ExerciseSetViewModel()
    {
        StartExerciseSetCommand = new Command(
            async () => await StartExerciseSetAsync());
    }

    // Receives navigation data when the page is opened.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("topicName", out var topicName))
        {
            TopicName = topicName?.ToString() ?? string.Empty;
        }
    }

    // Opens the first exercise for the selected topic.
    private async Task StartExerciseSetAsync()
    {
        if (string.IsNullOrWhiteSpace(TopicName))
        {
            return;
        }

        await Shell.Current.GoToAsync(
            nameof(ExercisePage),
            new Dictionary<string, object>
            {
                ["topicName"] = TopicName
            });
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}