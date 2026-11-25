// File: MatMentor.Mobile/Models/TopicListResponse.cs
namespace MatMentor.Mobile.Models;

public record TopicListResponse(IReadOnlyList<TopicItem> Topics);
public record TopicItem(int Id, string Name);
