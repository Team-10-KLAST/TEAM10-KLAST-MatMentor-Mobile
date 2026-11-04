
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using MatMentor.Mobile.Models;
using MatMentor.Mobile.Services;

namespace MatMentor.Mobile.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly LoginApiService _loginApiService;
    private readonly AuthSessionService _authSessionService;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoggingIn;

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

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoggingIn
    {
        get => _isLoggingIn;
        private set
        {
            if (_isLoggingIn == value)
                return;

            _isLoggingIn = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel(
        LoginApiService loginApiService,
        AuthSessionService authSessionService)
    {
        _loginApiService = loginApiService;
        _authSessionService = authSessionService;

        LoginCommand = new Command(
            async () => await LoginAsync());
    }

    // Authenticates the student and stores the JWT securely.
    private async Task LoginAsync()
    {
        if (IsLoggingIn)
            return;

        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Skriv dit brugernavn.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Skriv dit kodeord.";
            return;
        }

        IsLoggingIn = true;
        ErrorMessage = string.Empty;

        try
        {
            var request = new LoginRequest(
                Username.Trim(),
                Password);

            var token = await _loginApiService.LoginAsync(request);

            try
            {
                await _authSessionService.SaveTokenAsync(token);
            }
            catch (Exception)
            {
                ErrorMessage =
                    "Login blev godkendt, men kunne ikke gemmes sikkert på enheden.";
                return;
            }

            Password = string.Empty;

            await Shell.Current.GoToAsync("//StartPage");
        }
        catch (HttpRequestException ex)
            when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Brugernavn eller kodeord er forkert.";
        }
        catch (HttpRequestException ex)
            when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            ErrorMessage = "Kontrollér brugernavn og kodeord.";
        }
        catch (HttpRequestException ex)
            when (ex.StatusCode is not null)
        {
            ErrorMessage =
                "Serveren kunne ikke behandle login. Prøv igen senere.";
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Kan ikke få forbindelse til serveren. Kontrollér forbindelsen.";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Login tog for lang tid. Prøv igen.";
        }
        catch (JsonException)
        {
            ErrorMessage =
                "Serverens svar kunne ikke læses. Prøv igen senere.";
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    // Notifies the view when a bound property changes.
    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
