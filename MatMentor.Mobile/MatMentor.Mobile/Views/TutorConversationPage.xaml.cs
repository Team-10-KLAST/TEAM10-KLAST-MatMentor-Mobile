using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class TutorConversationPage : ContentPage
{
    public TutorConversationPage()
    {
        InitializeComponent();
        BindingContext = new TutorConversationViewModel();
    }
}