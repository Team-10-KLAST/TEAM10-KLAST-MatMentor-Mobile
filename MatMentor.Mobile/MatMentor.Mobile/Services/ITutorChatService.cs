using MatMentor.Mobile.Models;

namespace MatMentor.Mobile.Services;

public interface ITutorChatService
{
    Task<TutorChatResponse> AskAsync(
        TutorChatRequest request,
        CancellationToken cancellationToken = default);
}
