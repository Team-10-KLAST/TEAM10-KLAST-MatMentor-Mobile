namespace MatMentor.Mobile.Models;

public sealed record TutorChatRequest(
    string ExerciseText,
    string AttemptText,
    IReadOnlyList<TutorChatMessage> Messages);
