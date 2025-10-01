using BingeBuddy.Services;
using BingeBuddy.ViewModels;
using BingeBuddy.Views;
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

            // Register ViewModels as Singletons
            builder.Services.AddSingleton<GlobalSearchViewModel>();
            builder.Services.AddSingleton<DiscoverViewModel>();
            builder.Services.AddSingleton<MyShowsViewModel>();
            builder.Services.AddSingleton<WatchlistViewModel>();

            // Register Views
            builder.Services.AddTransient<DiscoverPage>();
            builder.Services.AddTransient<MyShowsPage>();
            builder.Services.AddTransient<WatchlistPage>();
            builder.Services.AddTransient<SearchResultsModal>();

            // Register App and AppShell
            builder.Services.AddSingleton<App>();
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder;
        }
    }
}