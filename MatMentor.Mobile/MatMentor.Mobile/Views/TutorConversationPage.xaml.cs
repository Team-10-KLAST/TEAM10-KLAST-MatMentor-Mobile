using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class TutorConversationPage : ContentPage
{
    private readonly TutorConversationViewModel _viewModel;

    public TutorConversationPage(TutorConversationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        viewModel.Messages.CollectionChanged += (_, _) =>
        {
            if (viewModel.Messages.Count == 0)
            {
                return;
            }

            var lastMessage = viewModel.Messages[^1];
            Dispatcher.Dispatch(() =>
            {
                ConversationCollectionView.ScrollTo(
                    lastMessage,
                    position: ScrollToPosition.End,
                    animate: false);
            });
        };
    }

    // Requests the initial tutor response when the page becomes visible.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ActivateConversation();
    }

    // Makes leaving the page end the conversation even when Android back is used.
    protected override void OnDisappearing()
    {
        _viewModel.CloseConversation();
        base.OnDisappearing();
    }
}
