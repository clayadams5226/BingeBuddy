using BingeBuddy.ViewModels;

namespace BingeBuddy.Views
{
    public partial class DiscoverPage : ContentPage
    {
        private readonly DiscoverViewModel _viewModel;

        public DiscoverPage(DiscoverViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load trending shows when page appears
            if (_viewModel.TrendingShows.Count == 0)
            {
                await _viewModel.LoadTrendingShowsCommand.ExecuteAsync(null);
            }
        }
    }
}