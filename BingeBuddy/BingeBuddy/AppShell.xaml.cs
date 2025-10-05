using BingeBuddy.Services;
using BingeBuddy.ViewModels;
using BingeBuddy.Views;

namespace BingeBuddy
{
    public partial class AppShell : Shell
    {
        private readonly GlobalSearchViewModel _searchViewModel;
        private readonly IServiceProvider _serviceProvider;

        public AppShell(GlobalSearchViewModel searchViewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _searchViewModel = searchViewModel;
            _serviceProvider = serviceProvider;
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            var searchModal = new SearchResultsModal(_searchViewModel, _serviceProvider);
            await Navigation.PushModalAsync(new NavigationPage(searchModal)
            {
                BarBackgroundColor = Color.FromArgb("#1A1A1A"),
                BarTextColor = Colors.White
            });
        }
    }
}