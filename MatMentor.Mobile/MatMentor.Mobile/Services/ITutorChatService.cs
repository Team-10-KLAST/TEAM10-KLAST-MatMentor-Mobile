using MatMentor.Mobile.Models;

namespace MatMentor.Mobile.Services;

public interface ITutorChatService
{
    Task<TutorChatResponse> StartAsync(TutorChatStartRequest request, CancellationToken cancellationToken = default);
    Task<TutorChatResponse> SendAsync(Guid conversationId, TutorChatSendRequest request, CancellationToken cancellationToken = default);
    Task EndAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
