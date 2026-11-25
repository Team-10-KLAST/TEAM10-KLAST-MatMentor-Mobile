// File: MatMentor.Mobile/ViewModels/ExerciseSetViewModel.cs
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using MatMentor.Mobile.Models;
using MatMentor.Mobile.Services;
using MatMentor.Mobile.Views;

namespace MatMentor.Mobile.ViewModels;

public class ExerciseSetViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private readonly IExerciseSetApiService _api;
    private string _topicName = string.Empty;
    private string _errorMessage = string.Empty;
    private int _topicId;
    private bool _isStarting;
    private StartExerciseSetResponse? _startedSet;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string TopicName
    {
        get => _topicName;
        private set { _topicName = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set { _errorMessage = value; OnPropertyChanged(); }
    }

    public bool IsStarting
    {
        get => _isStarting;
        private set
        {
            _isStarting = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanStart));
        }
    }

    public bool CanStart => !IsStarting;
    public ICommand StartExerciseSetCommand { get; }

    public ExerciseSetViewModel(IExerciseSetApiService api)
    {
        _api = api;
        StartExerciseSetCommand = new Command(async () => await StartExerciseSetAsync());
    }

    // Receives the topic identifier retrieved from the authenticated backend.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("topicName", out var topicName))
            TopicName = topicName?.ToString() ?? string.Empty;

        if (query.TryGetValue("topicId", out var topicId)
            && int.TryParse(topicId?.ToString(), out int parsedId))
            _topicId = parsedId;
    }

    // Creates an attempt once, then opens its exercises even after a navigation retry.
    private async Task StartExerciseSetAsync()
    {
        if (IsStarting)
            return;

        if (_topicId <= 0)
        {
            ErrorMessage = "Der er ikke valgt et gyldigt emne.";
            return;
        }

        IsStarting = true;
        ErrorMessage = string.Empty;
        try
        {
            _startedSet ??= await _api.StartAsync(_topicId);
            await Shell.Current.GoToAsync(nameof(ExercisePage),
                new Dictionary<string, object>
                {
                    ["topicName"] = TopicName,
                    ["exerciseSet"] = _startedSet
                });
        }
        catch (UnauthorizedAccessException)
        {
            ErrorMessage = "Du skal logge ind igen.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            ErrorMessage = "Der er ikke flere nye øvesæt i dette emne. Vælg et andet emne.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Din login-session er udløbet. Log ind igen.";
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Kunne ikke starte øvesættet. Kontrollér forbindelsen og prøv igen.";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Serveren svarer ikke. Prøv igen.";
        }
        catch (JsonException)
        {
            ErrorMessage = "Øvesættet kunne ikke læses fra serveren.";
        }
        finally
        {
            IsStarting = false;
        }
    }

    // Notifies the view when a bound property changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
