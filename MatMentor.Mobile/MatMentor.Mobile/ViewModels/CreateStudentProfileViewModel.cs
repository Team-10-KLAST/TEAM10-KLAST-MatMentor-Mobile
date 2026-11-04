
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MatMentor.Mobile.Models;
using MatMentor.Mobile.Services;
using System.Net;
using MatMentor.Mobile.Views;
using System.Net.Mail;

namespace MatMentor.Mobile.ViewModels;

public class CreateStudentProfileViewModel : INotifyPropertyChanged
{
    private readonly CreateStudentProfileApiService _apiService;

    private bool _isCreatingProfile;
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

    public bool IsCreatingProfile
    {
        get => _isCreatingProfile;
        private set
        {
            if (_isCreatingProfile == value)
            {
                return;
            }

            _isCreatingProfile = value;
            OnPropertyChanged();
        }
    }

    public ICommand CreateStudentProfileCommand { get; }

    public CreateStudentProfileViewModel(
        CreateStudentProfileApiService apiService)
    {
        _apiService = apiService;

        CreateStudentProfileCommand = new Command(
            async () => await ValidateProfileAsync());
    }

    // Validates the profile and prevents duplicate API requests.
    private async Task ValidateProfileAsync()
    {
        if (_isCreatingProfile)
        {
            return;
        }

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
            ErrorMessage = "Skriv en e-mail.";
            return;
        }

        var interestIds = new List<int>();

        if (IsFootballSelected)
        {
            interestIds.Add(1);
        }

        if (IsRidingSelected)
        {
            interestIds.Add(2);
        }

        ErrorMessage = string.Empty;

        var request = new CreateStudentProfileRequest(
            Username,
            Password,
            ParentEmail,
            Grade,
            interestIds
        );

        IsCreatingProfile = true;

        try
        {
            await _apiService.CreateAsync(request);

            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        catch (CreateStudentProfileValidationException ex)
        {
            var messages = ex.Errors
                .Select(error => error switch
                {
                    "Username is required." => "Skriv et brugernavn.",
                    "Password is required." => "Skriv et kodeord.",
                    "Grade must be between 7 and 9." => "Vælg 7., 8. eller 9. klasse.",
                    _ => "Oplysningerne blev afvist. Kontrollér brugernavn og kodeord."
                })
                .Distinct()
                .ToList();

            ErrorMessage = messages.Count > 0
                ? string.Join(Environment.NewLine, messages)
                : "Oplysningerne blev afvist. Prøv igen.";
        }

        catch (HttpRequestException ex)
            when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            ErrorMessage = "Oplysningerne blev afvist. Kontrollér dine oplysninger.";
        }
        catch (HttpRequestException ex)
            when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            ErrorMessage = "En valgt interesse blev ikke fundet.";
        }
        catch (HttpRequestException ex)
            when (ex.StatusCode is not null)
        {
            ErrorMessage = "Der opstod en fejl på serveren. Prøv igen senere.";
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Kan ikke få forbindelse til serveren. Kontrollér internetforbindelsen.";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Forbindelsen tog for lang tid. Prøv igen.";
        }

        finally
        {
            IsCreatingProfile = false;
        }
    }

    // Notifies the view when a property value changes.
    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
