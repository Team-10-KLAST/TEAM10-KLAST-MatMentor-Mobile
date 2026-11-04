using System.Net.Http.Json;
using System.Text.Json;
using MatMentor.Mobile.Models;

namespace MatMentor.Mobile.Services;

public sealed class LoginApiService
{
    private readonly HttpClient _httpClient;

    public LoginApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Authenticates the student and returns the JWT issued by the backend.
    public async Task<string> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(
            cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(result?.Token))
        {
            throw new JsonException(
                "The login response did not contain a token.");
        }

        return result.Token;
    }
}