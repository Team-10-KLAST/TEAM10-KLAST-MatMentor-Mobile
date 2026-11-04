
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using MatMentor.Mobile.Models;

namespace MatMentor.Mobile.Services;

public sealed class CreateStudentProfileApiService
{
    private readonly HttpClient _httpClient;

    public CreateStudentProfileApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Sends a student profile to the backend and handles validation errors.
    public async Task CreateAsync(CreateStudentProfileRequest request)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/students",
            request);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ApiErrorResponse? apiError = null;

            try
            {
                apiError = await response.Content
                    .ReadFromJsonAsync<ApiErrorResponse>();
            }
            catch (JsonException)
            {
                throw new CreateStudentProfileValidationException([]);
            }

            var errors = apiError?.Errors ?? new List<string>();
            var generalError = apiError?.Error;

            if (errors.Count == 0 &&
                !string.IsNullOrWhiteSpace(generalError))
            {
                errors.Add(generalError);
            }

            throw new CreateStudentProfileValidationException(errors);
        }

        response.EnsureSuccessStatusCode();
    }
}

public sealed class CreateStudentProfileValidationException
    : HttpRequestException
{
    public IReadOnlyList<string> Errors { get; }

    public CreateStudentProfileValidationException(
        IReadOnlyList<string> errors)
        : base(
            "The backend rejected the student profile.",
            null,
            HttpStatusCode.BadRequest)
    {
        Errors = errors.ToArray();
    }
}
