using BingeBuddy.Services;
using BingeBuddy.ViewModels;

namespace BingeBuddy
{
    public partial class App : Application
    {
        public App(GlobalSearchViewModel searchViewModel, DatabaseService databaseService)
        {
            InitializeComponent();

            MainPage = new AppShell(searchViewModel, databaseService);
        }
    }
}