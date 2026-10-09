using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MatMentor.Mobile.Views;
using System.Collections.ObjectModel;

namespace MatMentor.Mobile.ViewModels;

public class TutorConversationViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private string _exerciseText = string.Empty;
    private string _attemptText = string.Empty;
    private string _studentMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private string _conversationStatusMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ExerciseText
    {
        get => _exerciseText;
        private set
        {
            _exerciseText = value;
            OnPropertyChanged();
        }
    }

    public string AttemptText
    {
        get => _attemptText;
        private set
        {
            _attemptText = value;
            OnPropertyChanged();
        }
    }

    public string StudentMessage
    {
        get => _studentMessage;
        set
        {
            _studentMessage = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string ConversationStatusMessage
    {
        get => _conversationStatusMessage;
        private set
        {
            _conversationStatusMessage = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<TutorMessageViewModel> Messages { get; } = [];

    public ICommand SendMessageCommand { get; }

    public ICommand EndConversationCommand { get; }

    public TutorConversationViewModel()
    {
        SendMessageCommand = new Command(SendMessage);

        EndConversationCommand = new Command(
            async () => await EndConversationAsync());
    }

    // Receives the original exercise and the student's initial attempt.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("exerciseText", out var exerciseText))
        {
            ExerciseText = exerciseText?.ToString() ?? string.Empty;
        }

        if (query.TryGetValue("attemptText", out var attemptText))
        {
            AttemptText = attemptText?.ToString() ?? string.Empty;
        }
    }

    // Stores the student's message in the current temporary conversation.
    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(StudentMessage))
        {
            ErrorMessage = "Skriv en besked, før du sender.";
            return;
        }

        var messageText = StudentMessage.Trim();

        Messages.Add(
            new TutorMessageViewModel(
                "Dig",
                messageText));

        StudentMessage = string.Empty;
        ErrorMessage = string.Empty;

        ConversationStatusMessage =
            "Beskeden er sendt i den lokale prototype. AI er ikke koblet på endnu.";
    }

    // Ends the temporary tutor conversation and returns to topic selection.
    private async Task EndConversationAsync()
    {
        await Shell.Current.GoToAsync("../..");
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}