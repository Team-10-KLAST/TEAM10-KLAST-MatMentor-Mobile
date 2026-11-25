// File: MatMentor.Mobile/ViewModels/TopicSelectionViewModel.cs
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using MatMentor.Mobile.Services;
using MatMentor.Mobile.Views;

namespace MatMentor.Mobile.ViewModels;

public class TopicSelectionViewModel : INotifyPropertyChanged
{
    private readonly IExerciseSetApiService _api;
    private string _selectedTopicName = string.Empty;
    private string _selectionMessage = string.Empty;
    private bool _isLoading;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string SelectedTopicName
    {
        get => _selectedTopicName;
        private set { _selectedTopicName = value; OnPropertyChanged(); }
    }

    public string SelectionMessage
    {
        get => _selectionMessage;
        private set { _selectionMessage = value; OnPropertyChanged(); }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set { _isLoading = value; OnPropertyChanged(); }
    }

    public ICommand SelectTopicCommand { get; }

    public TopicSelectionViewModel(IExerciseSetApiService api)
    {
        _api = api;
        SelectTopicCommand = new Command<string>(async topic => await SelectTopicAsync(topic));
    }

    // Looks up the selected topic on the server rather than assuming fixed database IDs.
    private async Task SelectTopicAsync(string? topicName)
    {
        if (IsLoading || string.IsNullOrWhiteSpace(topicName))
            return;

        IsLoading = true;
        SelectionMessage = string.Empty;
        try
        {
            var response = await _api.GetTopicsAsync();
            var topic = response.Topics?.FirstOrDefault(t => t.Name == topicName);
            if (topic is null)
            {
                SelectionMessage = "Emnet findes ikke på serveren. Prøv igen senere.";
                return;
            }

            SelectedTopicName = topic.Name;
            await Shell.Current.GoToAsync(nameof(ExerciseSetPage),
                new Dictionary<string, object>
                {
                    ["topicId"] = topic.Id,
                    ["topicName"] = topic.Name
                });
        }
        catch (UnauthorizedAccessException)
        {
            SelectionMessage = "Du skal logge ind igen.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            SelectionMessage = "Din login-session er udløbet. Log ind igen.";
        }
        catch (HttpRequestException)
        {
            SelectionMessage = "Kan ikke hente emner fra serveren. Kontrollér forbindelsen.";
        }
        catch (TaskCanceledException)
        {
            SelectionMessage = "Serveren svarer ikke. Prøv igen.";
        }
        catch (JsonException)
        {
            SelectionMessage = "Serverens emneliste kunne ikke læses.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Notifies the view when a bound property changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
