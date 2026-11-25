// File: MatMentor.Mobile/Services/IExerciseSetApiService.cs
using MatMentor.Mobile.Models;

namespace MatMentor.Mobile.Services;

public interface IExerciseSetApiService
{
    Task<TopicListResponse> GetTopicsAsync(CancellationToken cancellationToken = default);
    Task<StartExerciseSetResponse> StartAsync(int topicId, CancellationToken cancellationToken = default);
    Task SubmitAnswerAsync(int attemptId, int exerciseId, string answer, CancellationToken cancellationToken = default);
    Task CompleteAsync(int attemptId, CancellationToken cancellationToken = default);
}
