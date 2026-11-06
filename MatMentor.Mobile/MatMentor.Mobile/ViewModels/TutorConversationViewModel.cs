using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using MatMentor.Mobile.Models;
using MatMentor.Mobile.Services;

namespace MatMentor.Mobile.ViewModels;

public class TutorConversationViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private const int MaxExerciseLength = 4000;
    private const int MaxAttemptLength = 2000;

    private readonly ITutorChatService _tutorChatService;
    private readonly TutorConversationHistory _history = new();
    private readonly CancellationTokenSource _conversationCancellation = new();

    private string _exerciseText = string.Empty;
    private string _attemptText = string.Empty;
    private string _studentMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private string _conversationStatusMessage = string.Empty;
    private bool _isBusy;
    private bool _showRetry;
    private bool _hasInitialReply;
    private bool _isActive;
    private bool _isClosed;

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

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value)
            {
                return;
            }

            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSendMessage));
        }
    }

    public bool ShowRetry
    {
        get => _showRetry;
        private set
        {
            _showRetry = value;
            OnPropertyChanged();
        }
    }

    public bool CanSendMessage => !_isClosed && _hasInitialReply && !IsBusy;

    public ObservableCollection<TutorMessageViewModel> Messages { get; } = [];

    public ICommand SendMessageCommand { get; }

    public ICommand RetryInitialCommand { get; }

    public ICommand EndConversationCommand { get; }

    public TutorConversationViewModel(ITutorChatService tutorChatService)
    {
        _tutorChatService = tutorChatService;

        SendMessageCommand = new Command(async () => await SendMessageAsync());
        RetryInitialCommand = new Command(async () => await StartConversationAsync());
        EndConversationCommand = new Command(async () => await EndConversationAsync());
    }

    // Receives the exercise and attempt as one-time Shell navigation parameters.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (_isClosed)
        {
            return;
        }

        if (query.TryGetValue("exerciseText", out var exerciseText))
        {
            ExerciseText = exerciseText?.ToString() ?? string.Empty;
        }

        if (query.TryGetValue("attemptText", out var attemptText))
        {
            AttemptText = attemptText?.ToString() ?? string.Empty;
        }

        if (_isActive)
        {
            _ = StartConversationAsync();
        }
    }

    // Marks the page as visible and starts the initial tutor request once the parameters arrive.
    public void ActivateConversation()
    {
        _isActive = true;
        _ = StartConversationAsync();
    }

    // Requests the first tutor reply with an empty conversation history.
    public async Task StartConversationAsync()
    {
        if (_isClosed || !_isActive || _hasInitialReply || IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ExerciseText) || string.IsNullOrWhiteSpace(AttemptText))
        {
            return;
        }

        if (ExerciseText.Length > MaxExerciseLength || AttemptText.Length > MaxAttemptLength)
        {
            ErrorMessage = "Opgaven eller dit første forsøg er for langt. Gå tilbage og forkort teksten.";
            return;
        }

        ShowRetry = false;
        ErrorMessage = string.Empty;
        ConversationStatusMessage = "Tutoren tænker...";
        IsBusy = true;

        try
        {
            var response = await _tutorChatService.AskAsync(
                new TutorChatRequest(ExerciseText, AttemptText, []),
                _conversationCancellation.Token);

            if (_isClosed)
            {
                return;
            }

            _history.BeginWithTutorReply(response.Reply);
            Messages.Add(new TutorMessageViewModel("Tutor", response.Reply));
            _hasInitialReply = true;
            OnPropertyChanged(nameof(CanSendMessage));
        }
        catch (OperationCanceledException) when (_isClosed)
        {
            return;
        }
        catch (Exception exception)
        {
            if (!_isClosed)
            {
                ErrorMessage = GetErrorMessage(exception);
                ShowRetry = true;
            }
        }
        finally
        {
            if (!_isClosed)
            {
                ConversationStatusMessage = string.Empty;
                IsBusy = false;
            }
        }
    }

    // Sends the student's message with complete prior assistant/user exchanges.
    private async Task SendMessageAsync()
    {
        if (!CanSendMessage)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(StudentMessage))
        {
            ErrorMessage = "Skriv en besked, før du sender.";
            return;
        }

        var messageText = StudentMessage.Trim();
        if (!_history.TryCreateStudentTurn(messageText, out var requestHistory, out var errorMessage))
        {
            ErrorMessage = errorMessage;
            return;
        }

        var message = new TutorMessageViewModel("Dig", messageText);

        ErrorMessage = string.Empty;
        ConversationStatusMessage = "Tutoren tænker...";
        Messages.Add(message);
        StudentMessage = string.Empty;
        IsBusy = true;

        try
        {
            var response = await _tutorChatService.AskAsync(
                new TutorChatRequest(ExerciseText, AttemptText, requestHistory),
                _conversationCancellation.Token);

            if (_isClosed)
            {
                return;
            }

            _history.CompleteStudentTurn(messageText, response.Reply);
            Messages.Add(new TutorMessageViewModel("Tutor", response.Reply));
        }
        catch (OperationCanceledException) when (_isClosed)
        {
            return;
        }
        catch (Exception exception)
        {
            if (!_isClosed)
            {
                Messages.Remove(message);
                StudentMessage = messageText;
                ErrorMessage = GetErrorMessage(exception);
            }
        }
        finally
        {
            if (!_isClosed)
            {
                ConversationStatusMessage = string.Empty;
                IsBusy = false;
            }
        }
    }

    // Clears the transient chat state and cancels any active AI request.
    public void CloseConversation()
    {
        if (_isClosed)
        {
            return;
        }

        _isClosed = true;
        _conversationCancellation.Cancel();
        _history.Clear();
        Messages.Clear();
        ExerciseText = string.Empty;
        AttemptText = string.Empty;
        StudentMessage = string.Empty;
        ErrorMessage = string.Empty;
        ConversationStatusMessage = string.Empty;
        ShowRetry = false;
        OnPropertyChanged(nameof(CanSendMessage));
    }

    // Ends the chat explicitly and returns to the previous screens.
    private async Task EndConversationAsync()
    {
        CloseConversation();
        await Shell.Current.GoToAsync("../..");
    }

    // Converts transport failures to short Danish messages for the student.
    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => "Du skal logge ind igen for at bruge AI-tutoren.",
            HttpRequestException { StatusCode: HttpStatusCode.Unauthorized } =>
                "Din login-session er udløbet. Log ind igen.",
            HttpRequestException { StatusCode: HttpStatusCode.TooManyRequests } =>
                "Du har sendt for mange beskeder. Vent et minut, og prøv igen.",
            HttpRequestException { StatusCode: HttpStatusCode.BadRequest } =>
                "Beskeden kunne ikke behandles. Kontrollér teksten, og prøv igen.",
            HttpRequestException { StatusCode: HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout } =>
                "AI-tutoren er ikke tilgængelig lige nu. Prøv igen senere.",
            TimeoutException => "AI-tutoren svarede ikke i tide. Prøv igen.",
            HttpRequestException =>
                "Kan ikke få forbindelse til serveren. Kontrollér din internetforbindelse.",
            JsonException => "AI-tutorens svar kunne ikke læses. Prøv igen.",
            _ => "Der opstod en fejl. Prøv igen senere."
        };
    }

    // Notifies the page when a bound property changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
