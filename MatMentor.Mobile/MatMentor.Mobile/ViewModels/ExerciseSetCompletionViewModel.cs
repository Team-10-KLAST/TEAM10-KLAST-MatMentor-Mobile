using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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
            async () => await ChooseTopicAsync());
    }

    // Receives the completed topic name.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("topicName", out var topicName))
        {
            TopicName = topicName?.ToString() ?? string.Empty;
        }
    }

    // Goes back to the existing topic selection page
    // instead of opening a brand new one.
    private async Task ChooseTopicAsync()
    {
        await Shell.Current.GoToAsync("../../..");
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}