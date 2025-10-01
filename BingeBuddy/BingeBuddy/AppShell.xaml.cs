using BingeBuddy.Services;
using BingeBuddy.ViewModels;
using BingeBuddy.Views;

namespace BingeBuddy
{
    public partial class AppShell : Shell
    {
        private readonly GlobalSearchViewModel _searchViewModel;
        private readonly DatabaseService _databaseService;

        public AppShell(GlobalSearchViewModel searchViewModel, DatabaseService databaseService)
        {
            InitializeComponent();
            _searchViewModel = searchViewModel;
            _databaseService = databaseService;
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            var searchModal = new SearchResultsModal(_searchViewModel, _databaseService);
            await Navigation.PushModalAsync(new NavigationPage(searchModal)
            {
                BarBackgroundColor = Color.FromArgb("#1A1A1A"),
                BarTextColor = Colors.White
            });
        }
    }
}