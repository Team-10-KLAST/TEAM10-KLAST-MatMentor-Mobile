namespace MatMentor.Mobile.ViewModels;

public class TutorMessageViewModel
{
    public string Sender { get; }

    public string Text { get; }

    public TutorMessageViewModel(string sender, string text)
    {
        Sender = sender;
        Text = text;
    }
}