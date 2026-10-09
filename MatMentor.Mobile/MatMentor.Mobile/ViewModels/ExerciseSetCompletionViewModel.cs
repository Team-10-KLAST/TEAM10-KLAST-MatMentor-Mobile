using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MatMentor.Mobile.Views;

namespace MatMentor.Mobile.ViewModels;

public class ExerciseSetCompletionViewModel : INotifyPropertyChanged, IQueryAttributable
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

    public ICommand ChooseTopicCommand { get; }

    public ExerciseSetCompletionViewModel()
    {
        ChooseTopicCommand = new Command(
            async () => await Shell.Current.GoToAsync(nameof(TopicSelectionPage)));
    }

    // Receives the topic that was completed.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("topicName", out var topicName))
        {
            TopicName = topicName?.ToString() ?? string.Empty;
        }
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}