namespace MatMentor.Mobile.Models;

public sealed record TutorChatStartRequest(string ExerciseText, string AttemptText, Guid RequestId);
