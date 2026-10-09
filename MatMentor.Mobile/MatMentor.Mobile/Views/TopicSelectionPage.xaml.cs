using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class TopicSelectionPage : ContentPage
{
    public TopicSelectionPage()
    {
        InitializeComponent();
        BindingContext = new TopicSelectionViewModel();
    }
}