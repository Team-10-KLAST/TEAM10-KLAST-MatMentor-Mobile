using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MatMentor.Mobile.Models;

namespace MatMentor.Mobile.Services;

public sealed class BackendTutorChatService : ITutorChatService
{
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(75);

    private readonly HttpClient _httpClient;
    private readonly AuthSessionService _authSessionService;

    public BackendTutorChatService(
        HttpClient httpClient,
        AuthSessionService authSessionService)
    {
        _httpClient = httpClient;
        _authSessionService = authSessionService;
    }

    // Sends the temporary conversation to the authenticated backend without persisting it.
    public async Task<TutorChatResponse> AskAsync(
        TutorChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var token = await _authSessionService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("The student must sign in before using the tutor.");
        }

        using var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "api/chat-tutor/ask")
        {
            Content = JsonContent.Create(request)
        };

        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(RequestTimeout);

        try
        {
            using var response = await _httpClient.SendAsync(
                requestMessage,
                HttpCompletionOption.ResponseHeadersRead,
                timeoutCts.Token);

            response.EnsureSuccessStatusCode();

            var reply = await response.Content.ReadFromJsonAsync<TutorChatResponse>(
                cancellationToken: timeoutCts.Token);

            if (string.IsNullOrWhiteSpace(reply?.Reply))
            {
                throw new JsonException("The tutor response was empty.");
            }

            return reply;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("The tutor request timed out.");
        }
    }
}
