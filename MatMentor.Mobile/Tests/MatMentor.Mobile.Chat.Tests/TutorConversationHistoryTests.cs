using MatMentor.Mobile.Services;
using Xunit;

namespace MatMentor.Mobile.Chat.Tests;

public sealed class TutorConversationHistoryTests
{
    [Fact]
    public void InitialTutorReply_AllowsFirstStudentTurn()
    {
        var history = new TutorConversationHistory();
        history.BeginWithTutorReply("Hvilke tal er vigtige?");

        var accepted = history.TryCreateStudentTurn("200 og 15", out var messages, out _);

        Assert.True(accepted);
        Assert.Equal(2, messages.Count);
        Assert.Equal("assistant", messages[0].Role);
        Assert.Equal("user", messages[1].Role);
        Assert.Equal("200 og 15", messages[1].Content);
    }

    [Fact]
    public void CompletedTurn_PreservesAlternatingRoles()
    {
        var history = new TutorConversationHistory();
        history.BeginWithTutorReply("Start med tallene.");
        history.CompleteStudentTurn("Jeg bruger procent.", "Hvorfor procent?");

        var accepted = history.TryCreateStudentTurn("15 af 100", out var messages, out _);

        Assert.True(accepted);
        Assert.Equal(new[] { "assistant", "user", "assistant", "user" },
            messages.Select(message => message.Role));
    }

    [Fact]
    public void FailedTurn_IsNotCommittedToHistory()
    {
        var history = new TutorConversationHistory();
        history.BeginWithTutorReply("Første spørgsmål.");

        history.TryCreateStudentTurn("Forsøg", out _, out _);
        history.TryCreateStudentTurn("Nyt forsøg", out var messages, out _);

        Assert.Equal(2, messages.Count);
        Assert.Equal("Nyt forsøg", messages[1].Content);
    }

    [Fact]
    public void EmptyOrLongMessage_IsRejected()
    {
        var history = new TutorConversationHistory();
        history.BeginWithTutorReply("Hej.");

        Assert.False(history.TryCreateStudentTurn(" ", out _, out _));
        Assert.False(history.TryCreateStudentTurn(new string('a', 1501), out _, out _));
    }

    [Fact]
    public void OversizedHistory_IsRejectedBeforeNetworkCall()
    {
        var history = new TutorConversationHistory();
        history.BeginWithTutorReply("Start.");

        for (var i = 0; i < 10; i++)
        {
            history.CompleteStudentTurn($"Student {i}", $"Tutor {i}");
        }

        Assert.False(history.TryCreateStudentTurn("Next", out _, out var error));
        Assert.Contains("for lang", error);
    }

    [Fact]
    public void Clear_PreventsTheOldConversationFromBeingContinued()
    {
        var history = new TutorConversationHistory();
        history.BeginWithTutorReply("Hej.");
        history.CompleteStudentTurn("Hej igen.", "Hvordan går det?");
        history.Clear();

        Assert.False(history.TryCreateStudentTurn("Genoptag", out _, out _));
        history.BeginWithTutorReply("Ny samtale.");
        Assert.True(history.TryCreateStudentTurn("Ny besked", out var messages, out _));
        Assert.Equal("Ny samtale.", messages[0].Content);
    }

    [Fact]
    public void AnOversizedTutorReply_BlocksFurtherContextSubmission()
    {
        var history = new TutorConversationHistory();
        history.BeginWithTutorReply(new string('a', 1501));

        Assert.False(history.TryCreateStudentTurn("Hej", out _, out var error));
        Assert.Contains("for lang", error);
    }
}
