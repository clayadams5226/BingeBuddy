using BingeBuddy.Services;
using BingeBuddy.ViewModels;

namespace BingeBuddy
{
    public partial class App : Application
    {
        public App(GlobalSearchViewModel searchViewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = new AppShell(searchViewModel, serviceProvider);
        }
    }
}