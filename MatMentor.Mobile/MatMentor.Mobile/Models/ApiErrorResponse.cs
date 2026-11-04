
using System.Text.Json.Serialization;

namespace MatMentor.Mobile.Models;

public sealed class ApiErrorResponse
{
    [JsonPropertyName("errors")]
    public List<string>? Errors { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
