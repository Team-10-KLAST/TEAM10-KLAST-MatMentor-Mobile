// File: MatMentor.Mobile/ViewModels/ExerciseViewModel.cs
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using MatMentor.Mobile.Data;
using MatMentor.Mobile.Models;
using MatMentor.Mobile.Services;
using MatMentor.Mobile.Views;
using Microsoft.Maui.Graphics;

namespace MatMentor.Mobile.ViewModels;

public class ExerciseViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private static readonly CultureInfo DanishCulture = new("da-DK");
    private readonly IExerciseSetApiService _api;
    private readonly List<ExerciseItem> _exercises = new();
    private readonly Dictionary<int, string> _draftAnswers = new();
    private readonly HashSet<int> _confirmedSubmissions = new();

    private StartExerciseSetResponse? _exerciseSet;
    private MissionPresentation? _mission;
    private string _topicName = string.Empty;
    private string _exerciseText = string.Empty;
    private string _submittedAnswer = string.Empty;
    private string _errorMessage = string.Empty;
    private int _currentExerciseIndex;
    private bool _isIntroVisible = true;
    private bool _isBusy;
    private bool _isRestoringAnswer;
    private bool _completionRequested;
    private bool _isCompleted;
    private bool _hasUncertainSubmission;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string TopicName
    {
        get => _topicName;
        private set { _topicName = value; OnPropertyChanged(); }
    }

    public string ExerciseSetTitle => _exerciseSet?.Title ?? string.Empty;
    public string MissionIntro => _mission?.Introduction ??
        "Missionens introduktion er ikke tilgængelig i denne appversion.";
    public string MissionImageSource => _mission?.ImageFile ?? string.Empty;
    public bool HasMissionImage => _mission is not null;
    public string MissionCategoryText => _mission is null
        ? TopicName.ToUpperInvariant()
        : $"{TopicName.ToUpperInvariant()} · {_mission.Grade}. KLASSE";
    public bool IsFootballMission => _mission?.Interest == "Fodbold";
    public bool IsStandardMission => _mission?.Interest != "Fodbold" && _mission?.Interest != "Ridning";
    public Color MissionAccentColor => Color.FromArgb(IsFootballMission ? "#0756D8" : IsStandardMission ? "#58665E" : "#6336FF");
    public Color MissionSoftColor => Color.FromArgb(IsFootballMission ? "#E5F1FF" : IsStandardMission ? "#F2F3EE" : "#F1ECFF");
    public Color MissionOutlineColor => Color.FromArgb(IsFootballMission ? "#CDE0F9" : IsStandardMission ? "#DCE2D9" : "#E5DEF9");

    public Color Step1Color => GetStepColor(0);
    public Color Step2Color => GetStepColor(1);
    public Color Step3Color => GetStepColor(2);
    public Color Step4Color => GetStepColor(3);
    public Color Step5Color => GetStepColor(4);
    public Color Step1TextColor => GetStepTextColor(0);
    public Color Step2TextColor => GetStepTextColor(1);
    public Color Step3TextColor => GetStepTextColor(2);
    public Color Step4TextColor => GetStepTextColor(3);
    public Color Step5TextColor => GetStepTextColor(4);

    public bool IsIntroVisible
    {
        get => _isIntroVisible;
        private set
        {
            _isIntroVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsExercisesVisible));
            OnPropertyChanged(nameof(PageTitle));
        }
    }

    public bool IsExercisesVisible => !IsIntroVisible;
    public string PageTitle => IsIntroVisible ? "Din mission" : "Opgave";

    public string ExerciseText
    {
        get => _exerciseText;
        private set { _exerciseText = value; OnPropertyChanged(); }
    }

    public string UnitText => _exercises.Count == 0
        ? string.Empty : _exercises[_currentExerciseIndex].Unit ?? string.Empty;

    public string SubmittedAnswer
    {
        get => _submittedAnswer;
        set
        {
            if (!_isRestoringAnswer && !CanEditAnswer)
                return;

            _submittedAnswer = value ?? string.Empty;
            if (!_isRestoringAnswer && _exercises.Count > 0)
                _draftAnswers[_exercises[_currentExerciseIndex].Id] = _submittedAnswer;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set { _errorMessage = value; OnPropertyChanged(); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanNavigate));
            OnPropertyChanged(nameof(CanLeave));
            OnPropertyChanged(nameof(CanEditAnswer));
        }
    }

    public bool CanNavigate => !IsBusy && !_hasUncertainSubmission && !_isCompleted;
    public bool CanLeave => !IsBusy && !_isCompleted;
    public bool CanEditAnswer => !IsBusy && !_completionRequested && !_hasUncertainSubmission && !_isCompleted
        && _exercises.Count > 0 && !_confirmedSubmissions.Contains(_exercises[_currentExerciseIndex].Id);
    public string ExercisePositionText => _exercises.Count == 0 ? string.Empty
        : $"Opgave {_currentExerciseIndex + 1} af {_exercises.Count}";
    public string NextButtonText => _completionRequested ? "Bekræft afslutning igen"
        : _currentExerciseIndex == _exercises.Count - 1 ? "Afslut øvesæt" : "Næste opgave";

    public ICommand BeginExercisesCommand { get; }
    public ICommand NextExerciseCommand { get; }
    public ICommand LeaveExerciseCommand { get; }

    public ExerciseViewModel(IExerciseSetApiService api)
    {
        _api = api;
        BeginExercisesCommand = new Command(BeginExercises);
        NextExerciseCommand = new Command(async () => await GoToNextExerciseAsync());
        LeaveExerciseCommand = new Command(async () => await LeaveExerciseSetAsync());
    }

    // Receives the attempt and the five exercises returned by the backend, with visual assets matched by title.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("exerciseSet", out object? value) || value is not StartExerciseSetResponse set)
        {
            ErrorMessage = "Øvesættet mangler. Gå tilbage og vælg et emne.";
            return;
        }

        TopicName = query.TryGetValue("topicName", out object? topicName)
            ? topicName?.ToString() ?? string.Empty : string.Empty;
        _exerciseSet = set;
        _mission = MissionPresentationCatalog.Find(set.Title);
        _exercises.Clear();
        _exercises.AddRange(set.Exercises.OrderBy(e => e.Position));
        _draftAnswers.Clear();
        _confirmedSubmissions.Clear();
        foreach (ExerciseItem exercise in _exercises)
            _draftAnswers[exercise.Id] = string.Empty;

        _currentExerciseIndex = 0;
        _hasUncertainSubmission = false;
        _completionRequested = false;
        _isCompleted = false;
        ErrorMessage = string.Empty;
        IsIntroVisible = true;
        OnPropertyChanged(nameof(ExerciseSetTitle));
        OnPropertyChanged(nameof(MissionIntro));
        OnPropertyChanged(nameof(MissionImageSource));
        OnPropertyChanged(nameof(HasMissionImage));
        OnPropertyChanged(nameof(MissionCategoryText));
        OnPropertyChanged(nameof(IsFootballMission));
        OnPropertyChanged(nameof(IsStandardMission));
        OnPropertyChanged(nameof(MissionAccentColor));
        OnPropertyChanged(nameof(MissionSoftColor));
        OnPropertyChanged(nameof(MissionOutlineColor));
        UpdateCurrentExercise();
    }

    // Shows the first exercise without creating a second backend attempt.
    private void BeginExercises()
    {
        if (_exerciseSet is null || _exercises.Count != 5)
            return;
        IsIntroVisible = false;
    }

    // Validates and submits the current answer before showing the next exercise.
    private async Task GoToNextExerciseAsync()
    {
        if (!CanNavigate || IsIntroVisible || _exerciseSet is null || _exercises.Count != 5)
            return;

        ErrorMessage = string.Empty;
        IsBusy = true;
        try
        {
            if (_completionRequested)
            {
                await CompleteAttemptAsync();
                return;
            }

            if (!TryValidateCurrentAnswer(out string answer))
                return;

            ExerciseItem exercise = _exercises[_currentExerciseIndex];
            bool isLastExercise = _currentExerciseIndex == _exercises.Count - 1;

            if (_confirmedSubmissions.Contains(exercise.Id))
            {
                _hasUncertainSubmission = true;
                ErrorMessage = "Svaret er allerede registreret. Det må ikke sendes igen.";
                return;
            }

            if (isLastExercise)
            {
                bool confirmed = await Shell.Current.DisplayAlert(
                    "Afslut øvesættet?",
                    "Dit sidste svar bliver gemt, og øvesættet afsluttes. Du kan ikke rette tidligere svar.",
                    "Afslut", "Bliv");
                if (!confirmed)
                    return;
            }

            await _api.SubmitAnswerAsync(_exerciseSet.AttemptId, exercise.Id, answer);
            _confirmedSubmissions.Add(exercise.Id);

            if (isLastExercise)
            {
                _completionRequested = true;
                OnPropertyChanged(nameof(NextButtonText));
                OnPropertyChanged(nameof(CanEditAnswer));
                await CompleteAttemptAsync();
            }
            else
            {
                _currentExerciseIndex++;
                UpdateCurrentExercise();
            }
        }
        catch (UnauthorizedAccessException)
        {
            _hasUncertainSubmission = !_completionRequested;
            ErrorMessage = "Du skal logge ind igen. Forsøget kan ikke fortsættes sikkert herfra.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _hasUncertainSubmission = !_completionRequested;
            ErrorMessage = "Din login-session er udløbet. Forsøget kan ikke fortsættes sikkert herfra.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest && !_completionRequested)
        {
            ErrorMessage = "Serveren afviste svaret. Indtast et gyldigt tal med komma i stedet for punktum.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict && !_completionRequested)
        {
            _hasUncertainSubmission = true;
            ErrorMessage = "Serveren har allerede et svar på denne opgave. Svaret kan ikke indsendes igen eller ændres.";
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            if (_completionRequested)
            {
                ErrorMessage = "Serveren kunne ikke bekræfte afslutningen. Dit sidste svar er gemt. Prøv 'Bekræft afslutning igen'.";
            }
            else
            {
                _hasUncertainSubmission = true;
                ErrorMessage = "Forbindelsen blev afbrudt. Svaret kan allerede være gemt. Det må ikke indsendes igen uden kontrol af serveren.";
            }
        }
        catch (JsonException)
        {
            ErrorMessage = "Serverens svar kunne ikke læses.";
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(CanNavigate));
            OnPropertyChanged(nameof(CanLeave));
            OnPropertyChanged(nameof(CanEditAnswer));
        }
    }

    // Confirms that the current answer follows the backend's Danish number format.
    private bool TryValidateCurrentAnswer(out string answer)
    {
        answer = SubmittedAnswer;
        if (string.IsNullOrWhiteSpace(answer))
        {
            ErrorMessage = "Skriv et svar, før du går videre.";
            return false;
        }

        if (answer.Contains('.') || !decimal.TryParse(answer.Trim(),
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            DanishCulture, out _))
        {
            ErrorMessage = "Svaret skal være et tal. Brug komma i stedet for punktum.";
            return false;
        }

        return true;
    }

    // Leaves the current attempt without returning to or changing submitted exercises.
    private async Task LeaveExerciseSetAsync()
    {
        if (!CanLeave || _exerciseSet is null)
            return;

        IsBusy = true;
        try
        {
            bool leave = await Shell.Current.DisplayAlert(
                "Forlad øvesættet?",
                "Svar, der allerede er sendt, er gemt i backend. Øvesættet er ikke afsluttet og kan ikke genoptages i v1. Vil du forlade det?",
                "Forlad", "Bliv");
            if (leave)
                await Shell.Current.GoToAsync("../..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Completes the same attempt after all five answers have been confirmed by the backend.
    private async Task CompleteAttemptAsync()
    {
        if (_exerciseSet is null || !_completionRequested)
            return;

        try
        {
            await _api.CompleteAsync(_exerciseSet.AttemptId);
            _isCompleted = true;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            _isCompleted = true;
        }

        if (_isCompleted)
            await Shell.Current.GoToAsync(nameof(ExerciseSetCompletionPage),
                new Dictionary<string, object> { ["topicName"] = TopicName });
    }

    // Shows the active exercise including line breaks and an editable draft answer.
    private void UpdateCurrentExercise()
    {
        ExerciseText = _exercises.Count == 0 ? string.Empty : _exercises[_currentExerciseIndex].ExerciseText;
        _isRestoringAnswer = true;
        try
        {
            SubmittedAnswer = _exercises.Count == 0 ? string.Empty
                : _draftAnswers[_exercises[_currentExerciseIndex].Id];
        }
        finally
        {
            _isRestoringAnswer = false;
        }

        OnPropertyChanged(nameof(UnitText));
        OnPropertyChanged(nameof(ExercisePositionText));
        OnPropertyChanged(nameof(NextButtonText));
        OnPropertyChanged(nameof(CanEditAnswer));
        OnPropertyChanged(nameof(Step1Color));
        OnPropertyChanged(nameof(Step2Color));
        OnPropertyChanged(nameof(Step3Color));
        OnPropertyChanged(nameof(Step4Color));
        OnPropertyChanged(nameof(Step5Color));
        OnPropertyChanged(nameof(Step1TextColor));
        OnPropertyChanged(nameof(Step2TextColor));
        OnPropertyChanged(nameof(Step3TextColor));
        OnPropertyChanged(nameof(Step4TextColor));
        OnPropertyChanged(nameof(Step5TextColor));
    }

    // Shades each exercise position according to the selected mission's color theme.
    private Color GetStepColor(int index)
    {
        return Color.FromArgb(index == _currentExerciseIndex ?
            (IsFootballMission ? "#0756D8" : IsStandardMission ? "#58665E" : "#6336FF") : "#E4E9F0");
    }

    // Keeps active progress markers readable in all three themes.
    private Color GetStepTextColor(int index)
    {
        return Color.FromArgb(index == _currentExerciseIndex ? "#FFFFFF" : "#4E5A73");
    }

    // Notifies bound controls when their display state changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
