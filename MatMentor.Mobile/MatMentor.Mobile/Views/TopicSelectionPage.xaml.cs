// File: MatMentor.Mobile/Views/TopicSelectionPage.xaml.cs
using MatMentor.Mobile.ViewModels;

namespace MatMentor.Mobile.Views;

public partial class TopicSelectionPage : ContentPage
{
    public TopicSelectionPage(TopicSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}