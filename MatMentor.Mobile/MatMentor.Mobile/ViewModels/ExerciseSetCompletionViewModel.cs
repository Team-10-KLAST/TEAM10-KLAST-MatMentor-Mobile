// File: MatMentor.Mobile/ViewModels/ExerciseSetCompletionViewModel.cs
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
        private set { _topicName = value; OnPropertyChanged(); }
    }

    public string ResultText => "Dine fem svar er gemt, og øvesættet er afsluttet.";
    public string UnansweredText => "Der vises ikke point eller facit, da backend ikke har et resultat-endpoint i v1.";

    public ICommand ChooseTopicCommand { get; }

    public ExerciseSetCompletionViewModel()
    {
        ChooseTopicCommand = new Command(async () => await ChooseTopicAsync());
    }

    // Reads the confirmed topic name after the server has completed the attempt.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("topicName", out object? topicName))
            TopicName = topicName?.ToString() ?? string.Empty;
    }

    // Returns to topic selection after a successfully confirmed completion.
    private async Task ChooseTopicAsync()
    {
        await Shell.Current.GoToAsync("../../..");
    }

    // Notifies the page when the displayed topic changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
