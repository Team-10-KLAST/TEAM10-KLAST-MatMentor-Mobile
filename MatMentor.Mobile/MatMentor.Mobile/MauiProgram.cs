
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


            builder.Services.AddTransient<CreateStudentProfileApiService>();
            builder.Services.AddTransient<CreateStudentProfileViewModel>();
            builder.Services.AddTransient<CreateStudentProfilePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
