// File: MatMentor.Mobile/Services/ExerciseSetApiService.cs
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MatMentor.Mobile.Models;

namespace MatMentor.Mobile.Services;

public sealed class ExerciseSetApiService : IExerciseSetApiService
{
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(20);
    private readonly HttpClient _httpClient;
    private readonly AuthSessionService _session;

    public ExerciseSetApiService(HttpClient httpClient, AuthSessionService session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    // Loads the authoritative topic identifiers from the backend.
    public async Task<TopicListResponse> GetTopicsAsync(CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "api/topics");
        return await ReadResponseAsync<TopicListResponse>(request, HttpStatusCode.OK, cancellationToken);
    }

    // Creates one owned attempt and retrieves its five exercises without correct answers.
    public async Task<StartExerciseSetResponse> StartAsync(int topicId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post, "api/exercise-set-attempts", new StartExerciseSetRequest(topicId));
        StartExerciseSetResponse result = await ReadResponseAsync<StartExerciseSetResponse>(
            request, HttpStatusCode.Created, cancellationToken);
        if (result.AttemptId <= 0 || result.ExerciseSetId <= 0 || result.Exercises is null || result.Exercises.Count != 5 || string.IsNullOrWhiteSpace(result.Title)
            || result.Exercises.Select(e => e.Id).Distinct().Count() != 5
            || result.Exercises.Any(e => e.Id <= 0 || e.Position < 1 || e.Position > 5 || string.IsNullOrWhiteSpace(e.ExerciseText))
            || result.Exercises.Select(e => e.Position).Distinct().Count() != 5)
            throw new JsonException("The exercise set response is invalid.");
        return result;
    }

    // Submits one final answer belonging to the currently owned attempt.
    public async Task SubmitAnswerAsync(int attemptId, int exerciseId, string answer, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post, $"api/exercise-set-attempts/{attemptId}/answers",
            new SubmitAnswerRequest(exerciseId, answer));
        await SendNoContentAsync(request, cancellationToken);
    }

    // Completes the attempt after the five answers have been submitted.
    public async Task CompleteAsync(int attemptId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post, $"api/exercise-set-attempts/{attemptId}/completion");
        await SendNoContentAsync(request, cancellationToken);
    }

    // Creates an isolated HTTP request with the current token from secure storage.
    private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string path, object? body = null)
    {
        string? token = await _session.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("The student must sign in first.");

        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null)
            request.Content = JsonContent.Create(body);
        return request;
    }

    // Reads and validates a typed JSON response without exposing credentials or answers in logs.
    private async Task<T> ReadResponseAsync<T>(
        HttpRequestMessage request, HttpStatusCode expectedStatus, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(RequestTimeout);
        using var response = await _httpClient.SendAsync(request, timeout.Token);
        response.EnsureSuccessStatusCode();
        if (response.StatusCode != expectedStatus)
            throw new HttpRequestException($"Unexpected HTTP status {(int)response.StatusCode}.");

        T? result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: timeout.Token);
        return result ?? throw new JsonException("The API returned an empty response.");
    }

    // Confirms the existing 204 response contract for answer and completion commands.
    private async Task SendNoContentAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(RequestTimeout);
        using var response = await _httpClient.SendAsync(request, timeout.Token);
        response.EnsureSuccessStatusCode();
        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new HttpRequestException($"Unexpected HTTP status {(int)response.StatusCode}.");
    }
}
