using MatMentor.Mobile.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MatMentor.Mobile.ViewModels;

public class TutorViewModel : INotifyPropertyChanged
{
    private string _exerciseText = string.Empty;
    private string _attemptText = string.Empty;
    private string _errorMessage = string.Empty;
    private string _statusMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ExerciseText
    {
        get => _exerciseText;
        set
        {
            _exerciseText = value;
            OnPropertyChanged();
        }
    }

    public string AttemptText
    {
        get => _attemptText;
        set
        {
            _attemptText = value;
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

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand StartTutorCommand { get; }

    public TutorViewModel()
    {
        StartTutorCommand = new Command(
            async () => await StartTutorAsync());
    }

    // Validates the tutor input and opens the conversation page.
    private async Task StartTutorAsync()
    {
        if (string.IsNullOrWhiteSpace(ExerciseText))
        {
            ErrorMessage = "Indsæt den matematikopgave, du vil have hjælp til.";
            StatusMessage = string.Empty;
            return;
        }

        if (string.IsNullOrWhiteSpace(AttemptText))
        {
            ErrorMessage = "Fortæl, hvad du allerede har prøvet.";
            StatusMessage = string.Empty;
            return;
        }

        ErrorMessage = string.Empty;
        StatusMessage = string.Empty;

        var encodedExerciseText = Uri.EscapeDataString(ExerciseText);
        var encodedAttemptText = Uri.EscapeDataString(AttemptText);

        await Shell.Current.GoToAsync(
            $"{nameof(TutorConversationPage)}" +
            $"?exerciseText={encodedExerciseText}" +
            $"&attemptText={encodedAttemptText}");
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}