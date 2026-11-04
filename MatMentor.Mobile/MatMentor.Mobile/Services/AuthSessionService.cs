using Microsoft.Maui.Storage;

namespace MatMentor.Mobile.Services;

public sealed class AuthSessionService
{
    private const string TokenKey = "matmentor_student_jwt";
    private readonly ISecureStorage _secureStorage;

    public AuthSessionService(ISecureStorage secureStorage)
    {
        _secureStorage = secureStorage;
    }

    // Stores the signed token using the operating system's secure storage.
    public Task SaveTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException(
                "A token is required.",
                nameof(token));
        }

        return _secureStorage.SetAsync(TokenKey, token);
    }

    // Reads the token needed by authenticated backend calls.
    public Task<string?> GetTokenAsync()
    {
        return _secureStorage.GetAsync(TokenKey);
    }

    // Removes the student's token during logout.
    public void ClearToken()
    {
        _secureStorage.Remove(TokenKey);
    }
}