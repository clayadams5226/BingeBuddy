using BingeBuddy.Services;
using Microsoft.Extensions.Logging;

namespace BingeBuddy
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
        {
            // Register Services
            builder.Services.AddSingleton<ITVShowApiService, TMDbApiService>();
            builder.Services.AddSingleton<DatabaseService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder;
        }
    }
}