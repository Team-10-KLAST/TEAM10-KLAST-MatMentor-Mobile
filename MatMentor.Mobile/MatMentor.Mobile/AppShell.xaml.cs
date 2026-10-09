using MatMentor.Mobile.Views;

namespace MatMentor.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

        Routing.RegisterRoute(
            nameof(CreateStudentProfilePage),
            typeof(CreateStudentProfilePage));

        Routing.RegisterRoute(
            nameof(TopicSelectionPage),
            typeof(TopicSelectionPage));

        Routing.RegisterRoute(
            nameof(ExerciseSetPage),
            typeof(ExerciseSetPage));

        Routing.RegisterRoute(
            nameof(ExercisePage),
            typeof(ExercisePage));

        Routing.RegisterRoute(
            nameof(ExerciseSetCompletionPage),
            typeof(ExerciseSetCompletionPage));

        Routing.RegisterRoute(
            nameof(TutorPage),
            typeof(TutorPage));

        Routing.RegisterRoute(
            nameof(TutorConversationPage),
            typeof(TutorConversationPage));
    }
}