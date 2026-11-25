// File: MatMentor.Mobile/Models/StartExerciseSetResponse.cs
namespace MatMentor.Mobile.Models;

public record StartExerciseSetResponse(
    int AttemptId,
    int ExerciseSetId,
    string Title,
    IReadOnlyList<ExerciseItem> Exercises);

public record ExerciseItem(int Id, int Position, string ExerciseText, string? Unit);
