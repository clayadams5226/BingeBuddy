using BingeBuddy.Services;
using BingeBuddy.ViewModels;

namespace BingeBuddy
{
    public partial class App : Application
    {
        public App(GlobalSearchViewModel searchViewModel, DatabaseService databaseService, ITVShowApiService apiService)
        {
            InitializeComponent();

            MainPage = new AppShell(searchViewModel, databaseService, apiService);
        }
    }
}