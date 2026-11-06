using MatMentor.Mobile.Views;
using MatMentor.Mobile.ViewModels;
using MatMentor.Mobile.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace MatMentor.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


#if DEBUG
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:55077/")
            });
#else
#error Configure the HTTPS API address before building Release.
#endif

            builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);
            builder.Services.AddSingleton<AuthSessionService>();
            builder.Services.AddTransient<LoginApiService>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<StartViewModel>();
            builder.Services.AddTransient<StartPage>();

            builder.Services.AddTransient<CreateStudentProfileApiService>();
            builder.Services.AddTransient<CreateStudentProfileViewModel>();
            builder.Services.AddTransient<CreateStudentProfilePage>();

            builder.Services.AddTransient<ITutorChatService, BackendTutorChatService>();
            builder.Services.AddTransient<TutorViewModel>();
            builder.Services.AddTransient<TutorPage>();
            builder.Services.AddTransient<TutorConversationViewModel>();
            builder.Services.AddTransient<TutorConversationPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
