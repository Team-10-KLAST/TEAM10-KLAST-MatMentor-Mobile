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
    private const int MaxMessageLength = 1500;

    private readonly ITutorChatService _tutorChatService;
    private readonly CancellationTokenSource _conversationCancellation = new();

    private string _exerciseText = string.Empty;
    private string _attemptText = string.Empty;
    private string _studentMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private string _conversationStatusMessage = string.Empty;
    private Guid? _conversationId;
    private readonly Guid _startRequestId = Guid.NewGuid();
    private int _version;
    private Guid? _pendingMessageId;
    private string? _pendingMessageText;
    private bool _isBusy;
    private bool _showRetry;
    private bool _isActive;
    private bool _isClosed;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ExerciseText
    {
        get => _exerciseText;
        private set { _exerciseText = value; OnPropertyChanged(); }
    }

    public string AttemptText
    {
        get => _attemptText;
        private set { _attemptText = value; OnPropertyChanged(); }
    }

    public string StudentMessage
    {
        get => _studentMessage;
        set { _studentMessage = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set { _errorMessage = value; OnPropertyChanged(); }
    }

    public string ConversationStatusMessage
    {
        get => _conversationStatusMessage;
        private set { _conversationStatusMessage = value; OnPropertyChanged(); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSendMessage));
        }
    }

    public bool ShowRetry
    {
        get => _showRetry;
        private set { _showRetry = value; OnPropertyChanged(); }
    }

    public bool CanSendMessage => !_isClosed && _conversationId.HasValue && !IsBusy;
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

    // Receives the exercise and initial attempt from one-time MAUI Shell navigation parameters.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (_isClosed) return;
        if (query.TryGetValue("exerciseText", out var exerciseText))
            ExerciseText = exerciseText?.ToString() ?? string.Empty;
        if (query.TryGetValue("attemptText", out var attemptText))
            AttemptText = attemptText?.ToString() ?? string.Empty;
        if (_isActive) _ = StartConversationAsync();
    }

    // Starts the first server-side tutor conversation after the page becomes visible.
    public void ActivateConversation()
    {
        _isActive = true;
        _ = StartConversationAsync();
    }

    // Creates a session and stores only its conversation id and current version on the client.
    public async Task StartConversationAsync()
    {
        if (_isClosed || !_isActive || _conversationId.HasValue || IsBusy) return;
        if (string.IsNullOrWhiteSpace(ExerciseText) || string.IsNullOrWhiteSpace(AttemptText)) return;
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
            TutorChatResponse response = await _tutorChatService.StartAsync(
                new TutorChatStartRequest(ExerciseText, AttemptText, _startRequestId), _conversationCancellation.Token);
            if (_isClosed)
            {
                await TryEndOrphanAsync(response.ConversationId);
                return;
            }
            _conversationId = response.ConversationId;
            _version = response.Version;
            Messages.Add(new TutorMessageViewModel("Tutor", response.Reply));
            OnPropertyChanged(nameof(CanSendMessage));
        }
        catch (OperationCanceledException) when (_isClosed) { }
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
                IsBusy = false;
                ConversationStatusMessage = string.Empty;
            }
        }
    }

    // Sends a versioned student message with a stable id for safe retries after network errors.
    private async Task SendMessageAsync()
    {
        if (!CanSendMessage) return;
        string messageText = (StudentMessage ?? string.Empty).Trim();
        if (messageText.Length == 0)
        {
            ErrorMessage = "Skriv en besked, før du sender.";
            return;
        }
        if (messageText.Length > MaxMessageLength)
        {
            ErrorMessage = "Beskeden er for lang. Skriv højst 1500 tegn.";
            return;
        }
        if (_pendingMessageId is null || _pendingMessageText != messageText)
        {
            _pendingMessageId = Guid.NewGuid();
            _pendingMessageText = messageText;
        }
        var message = new TutorMessageViewModel("Dig", messageText);
        ErrorMessage = string.Empty;
        ConversationStatusMessage = "Tutoren tænker...";
        Messages.Add(message);
        StudentMessage = string.Empty;
        IsBusy = true;
        try
        {
            TutorChatResponse response = await _tutorChatService.SendAsync(_conversationId!.Value,
                new TutorChatSendRequest(messageText, _version, _pendingMessageId.Value),
                _conversationCancellation.Token);
            if (_isClosed) return;
            _version = response.Version;
            _pendingMessageId = null;
            _pendingMessageText = null;
            Messages.Add(new TutorMessageViewModel("Tutor", response.Reply));
        }
        catch (OperationCanceledException) when (_isClosed) { }
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
                IsBusy = false;
                ConversationStatusMessage = string.Empty;
            }
        }
    }

    // Clears the client UI and requests remote deletion, even when Android back navigation is used.
    public void CloseConversation()
    {
        if (_isClosed) return;
        _isClosed = true;
        _conversationCancellation.Cancel();
        Guid? conversationId = _conversationId;
        _conversationId = null;
        _ = conversationId.HasValue ? TryEndOrphanAsync(conversationId.Value) : Task.CompletedTask;
        Messages.Clear();
        ExerciseText = string.Empty;
        AttemptText = string.Empty;
        StudentMessage = string.Empty;
        ErrorMessage = string.Empty;
        ConversationStatusMessage = string.Empty;
        ShowRetry = false;
        OnPropertyChanged(nameof(CanSendMessage));
    }

    // Sends a best-effort delete with a fresh token, without restoring a closed conversation.
    private async Task TryEndOrphanAsync(Guid conversationId)
    {
        try { await _tutorChatService.EndAsync(conversationId); }
        catch (Exception) { }
    }

    // Ends the conversation explicitly and navigates back to the topic selection screen.
    private async Task EndConversationAsync()
    {
        CloseConversation();
        await Shell.Current.GoToAsync("../..");
    }

    // Maps API failures and stale sessions to readable Danish feedback.
    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => "Du skal logge ind igen for at bruge AI-tutoren.",
            HttpRequestException { StatusCode: HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden } =>
                "Din login-session er udløbet. Log ind igen.",
            HttpRequestException { StatusCode: HttpStatusCode.NotFound } =>
                "Samtalen er udløbet eller afsluttet. Gå tilbage og start en ny.",
            HttpRequestException { StatusCode: HttpStatusCode.Conflict } =>
                "Samtalen er optaget eller ændret. Prøv igen om et øjeblik.",
            HttpRequestException { StatusCode: HttpStatusCode.TooManyRequests } =>
                "Du har sendt for mange beskeder eller har for mange samtaler åbne. Vent lidt.",
            HttpRequestException { StatusCode: HttpStatusCode.BadRequest } =>
                "Beskeden kunne ikke behandles. Kontrollér teksten, og prøv igen.",
            HttpRequestException { StatusCode: HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout } =>
                "AI-tutoren er ikke tilgængelig lige nu. Prøv igen senere.",
            TimeoutException => "AI-tutoren svarede ikke i tide. Prøv igen.",
            HttpRequestException => "Kan ikke få forbindelse til serveren. Kontrollér internetforbindelsen.",
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
