namespace MatMentor.Mobile.Models;

public sealed record TutorChatSendRequest(string Content, int ExpectedVersion, Guid MessageId);
