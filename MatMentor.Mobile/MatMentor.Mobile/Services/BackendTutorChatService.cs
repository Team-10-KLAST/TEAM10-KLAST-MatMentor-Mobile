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

    public BackendTutorChatService(HttpClient httpClient, AuthSessionService authSessionService)
    {
        _httpClient = httpClient;
        _authSessionService = authSessionService;
    }

    // Creates the temporary server-side tutor session and retrieves its first reply.
    public Task<TutorChatResponse> StartAsync(TutorChatStartRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync(HttpMethod.Post, "api/tutor-conversations", request, cancellationToken);
    }

    // Sends only the new student message, without a client-supplied conversation history.
    public Task<TutorChatResponse> SendAsync(Guid conversationId, TutorChatSendRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync(HttpMethod.Post, $"api/tutor-conversations/{conversationId}/messages", request, cancellationToken);
    }

    // Deletes the owned conversation on the server without reusing the canceled chat token.
    public async Task EndAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Delete,
            $"api/tutor-conversations/{conversationId}", null);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(RequestTimeout);
        using var response = await _httpClient.SendAsync(request, timeout.Token);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return;
        response.EnsureSuccessStatusCode();
    }

    // Sends one JSON request and deserializes the typed tutor response.
    private async Task<TutorChatResponse> SendAsync(HttpMethod method, string path, object content, CancellationToken cancellationToken)
    {
        using var request = await CreateAuthorizedRequestAsync(method, path, content);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(RequestTimeout);
        try
        {
            using var response = await _httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead, timeout.Token);
            response.EnsureSuccessStatusCode();
            TutorChatResponse? body = await response.Content.ReadFromJsonAsync<TutorChatResponse>(cancellationToken: timeout.Token);
            if (body is null || body.ConversationId == Guid.Empty || string.IsNullOrWhiteSpace(body.Reply))
                throw new JsonException("Invalid tutor response.");
            return body;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("The tutor request timed out.");
        }
    }

    // Attaches the existing login token to one request instead of modifying global HttpClient headers.
    private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string path, object? content)
    {
        string? token = await _authSessionService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("The student must sign in first.");
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (content is not null)
            request.Content = JsonContent.Create(content);
        return request;
    }
}
