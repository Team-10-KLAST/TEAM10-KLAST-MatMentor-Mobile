using MatMentor.Mobile.Models;

namespace MatMentor.Mobile.Services;

public sealed class TutorConversationHistory
{
    private const int MaxMessageLength = 1500;
    private const int MaxMessageCount = 20;
    private const int MaxHistoryLength = 12000;

    private readonly List<TutorChatMessage> _messages = [];

    // Stores the tutor's initial reply as the first message in the active conversation.
    public void BeginWithTutorReply(string reply)
    {
        if (_messages.Count != 0 || string.IsNullOrWhiteSpace(reply))
        {
            throw new InvalidOperationException("The tutor conversation has an invalid starting state.");
        }

        _messages.Add(new TutorChatMessage("assistant", reply));
    }

    // Validates and builds a request without committing the student's message before AI succeeds.
    public bool TryCreateStudentTurn(
        string studentMessage,
        out IReadOnlyList<TutorChatMessage> requestMessages,
        out string errorMessage)
    {
        requestMessages = [];
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(studentMessage))
        {
            errorMessage = "Skriv en besked, før du sender.";
            return false;
        }

        var text = studentMessage.Trim();
        if (text.Length > MaxMessageLength)
        {
            errorMessage = "Beskeden er for lang. Skriv højst 1500 tegn.";
            return false;
        }

        if (_messages.Count == 0 || _messages.Count % 2 == 0)
        {
            errorMessage = "Samtalen er ikke klar til en ny besked.";
            return false;
        }

        if (_messages.Count + 1 > MaxMessageCount ||
            _messages.Any(message => message.Content.Length > MaxMessageLength) ||
            _messages.Sum(message => message.Content.Length) + text.Length > MaxHistoryLength)
        {
            errorMessage = "Samtalen er blevet for lang. Afslut den og start en ny samtale.";
            return false;
        }

        requestMessages = _messages
            .Append(new TutorChatMessage("user", text))
            .ToArray();
        return true;
    }

    // Commits a completed assistant/user exchange after a successful AI response.
    public void CompleteStudentTurn(string studentMessage, string tutorReply)
    {
        if (_messages.Count == 0 || _messages.Count % 2 == 0 ||
            string.IsNullOrWhiteSpace(studentMessage) || string.IsNullOrWhiteSpace(tutorReply))
        {
            throw new InvalidOperationException("The tutor conversation is not ready for an exchange.");
        }

        _messages.Add(new TutorChatMessage("user", studentMessage.Trim()));
        _messages.Add(new TutorChatMessage("assistant", tutorReply));
    }

    // Removes all temporary conversation context after the student leaves the chat.
    public void Clear()
    {
        _messages.Clear();
    }
}
