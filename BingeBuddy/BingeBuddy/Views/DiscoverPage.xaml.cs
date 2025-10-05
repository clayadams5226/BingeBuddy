using BingeBuddy.Models;
using BingeBuddy.ViewModels;

namespace BingeBuddy.Views
{
    public partial class DiscoverPage : ContentPage
    {
        private readonly DiscoverViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;

        public DiscoverPage(DiscoverViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _serviceProvider = serviceProvider;
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

        private async void OnShowTapped(object sender, EventArgs e)
        {
            if (sender is VerticalStackLayout layout && layout.BindingContext is Show show)
            {
                // Get ShowDetailPage and ViewModel from DI
                var detailPage = _serviceProvider.GetRequiredService<ShowDetailPage>();
                var detailViewModel = _serviceProvider.GetRequiredService<ShowDetailViewModel>();

                // Initialize the ViewModel with the show data
                await detailViewModel.InitializeAsync(show);

                // Set binding context
                detailPage.BindingContext = detailViewModel;

                await Navigation.PushAsync(detailPage);
            }
        }
    }
}