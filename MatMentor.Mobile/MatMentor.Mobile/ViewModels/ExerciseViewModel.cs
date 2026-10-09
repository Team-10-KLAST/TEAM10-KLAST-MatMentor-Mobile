using MatMentor.Mobile.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MatMentor.Mobile.Data;
using System.Collections.Generic;

namespace MatMentor.Mobile.ViewModels;

public class ExerciseViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private readonly List<string> _exerciseTexts = [];
    private readonly List<string> _submittedAnswers = [];

    private string _topicName = string.Empty;
    private string _exerciseText = string.Empty;
    private string _submittedAnswer = string.Empty;
    private string _errorMessage = string.Empty;
    private int _currentExerciseIndex;

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

    public string ExerciseText
    {
        get => _exerciseText;
        private set
        {
            _exerciseText = value;
            OnPropertyChanged();
        }
    }

    public string SubmittedAnswer
    {
        get => _submittedAnswer;
        set
        {
            _submittedAnswer = value;
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

    public string ExercisePositionText =>
        _exerciseTexts.Count == 0
            ? string.Empty
            : $"Opgave {_currentExerciseIndex + 1} af {_exerciseTexts.Count}";

    public string NextButtonText =>
        _currentExerciseIndex == _exerciseTexts.Count - 1
            ? "Afslut øvesæt"
            : "Næste opgave";

    public ICommand NextExerciseCommand { get; }

    public ExerciseViewModel()
    {
        NextExerciseCommand = new Command(
            async () => await GoToNextExerciseAsync());
    }

    // Receives the selected topic and prepares the exercise set.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("topicName", out var topicName))
        {
            return;
        }

        TopicName = topicName?.ToString() ?? string.Empty;

        LoadExercises();
    }

    // Loads temporary exercise data for the selected topic.
    private void LoadExercises()
    {
        _exerciseTexts.Clear();
        _submittedAnswers.Clear();

        List<string> exercises =
            ExercisePrototypeData.GetExercises(TopicName);

        _exerciseTexts.AddRange(exercises);

        _currentExerciseIndex = 0;
        SubmittedAnswer = string.Empty;
        ErrorMessage = string.Empty;

        UpdateCurrentExercise();
    }

    // Stores the current answer locally and moves to the next exercise or completion page.
    private async Task GoToNextExerciseAsync()
    {
        if (string.IsNullOrWhiteSpace(SubmittedAnswer))
        {
            ErrorMessage = "Skriv et svar, før du går videre.";
            return;
        }

        if (_submittedAnswers.Count > _currentExerciseIndex)
        {
            _submittedAnswers[_currentExerciseIndex] = SubmittedAnswer.Trim();
        }
        else
        {
            _submittedAnswers.Add(SubmittedAnswer.Trim());
        }

        ErrorMessage = string.Empty;

        if (_currentExerciseIndex == _exerciseTexts.Count - 1)
        {
            await Shell.Current.GoToAsync(
                nameof(ExerciseSetCompletionPage),
                new Dictionary<string, object>
                {
                    ["topicName"] = TopicName
                });

            return;
        }

        _currentExerciseIndex++;

        SubmittedAnswer = string.Empty;

        UpdateCurrentExercise();
    }

    // Updates the exercise displayed to the student.
    private void UpdateCurrentExercise()
    {
        if (_exerciseTexts.Count == 0)
        {
            ExerciseText = string.Empty;
            return;
        }

        ExerciseText = _exerciseTexts[_currentExerciseIndex];

        OnPropertyChanged(nameof(ExercisePositionText));
        OnPropertyChanged(nameof(NextButtonText));
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}