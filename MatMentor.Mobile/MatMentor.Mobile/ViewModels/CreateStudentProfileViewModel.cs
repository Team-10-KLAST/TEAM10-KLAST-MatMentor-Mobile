using MatMentor.Mobile.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MatMentor.Mobile.ViewModels;

public class CreateStudentProfileViewModel : INotifyPropertyChanged
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _parentEmail = string.Empty;
    private int _grade = 7;
    private bool _isFootballSelected;
    private bool _isRidingSelected;
    private string _errorMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public string ParentEmail
    {
        get => _parentEmail;
        set
        {
            _parentEmail = value;
            OnPropertyChanged();
        }
    }

    public List<int> Grades { get; } = [7, 8, 9];

    public int Grade
    {
        get => _grade;
        set
        {
            _grade = value;
            OnPropertyChanged();
        }
    }

    public bool IsFootballSelected
    {
        get => _isFootballSelected;
        set
        {
            _isFootballSelected = value;
            OnPropertyChanged();
        }
    }

    public bool IsRidingSelected
    {
        get => _isRidingSelected;
        set
        {
            _isRidingSelected = value;
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

    public ICommand CreateStudentProfileCommand { get; }

    public CreateStudentProfileViewModel()
    {
        CreateStudentProfileCommand = new Command(
            async () => await ValidateProfileAsync());
    }

    // Validates the required profile input and continues to topic selection.
    private async Task ValidateProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Skriv et brugernavn.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Skriv et kodeord.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ParentEmail))
        {
            ErrorMessage = "Skriv en forælders e-mail.";
            return;
        }

        ErrorMessage = string.Empty;

        await Shell.Current.GoToAsync(nameof(TopicSelectionPage));
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}