using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class TutorConversationPage : ContentPage
{
    public TutorConversationPage()
    {
        InitializeComponent();

        var viewModel = new TutorConversationViewModel();
        BindingContext = viewModel;

        viewModel.Messages.CollectionChanged += (_, _) =>
        {
            if (viewModel.Messages.Count == 0)
            {
                return;
            }

            var lastMessage = viewModel.Messages[^1];

            Dispatcher.Dispatch(async () =>
            {
                await Task.Delay(250);

                ConversationCollectionView.ScrollTo(
                    lastMessage,
                    position: ScrollToPosition.End,
                    animate: false);

                await Task.Delay(150);

                ConversationCollectionView.ScrollTo(
                    lastMessage,
                    position: ScrollToPosition.End,
                    animate: false);
            });
        };
    }
}