using BingeBuddy.ViewModels;

namespace BingeBuddy.Views
{
    public partial class WatchlistPage : ContentPage
    {
        private readonly WatchlistViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;

        public WatchlistPage(WatchlistViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _serviceProvider = serviceProvider;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadWatchlistCommand.ExecuteAsync(null);
        }

        private async void OnEpisodeTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is WatchlistItem item)
            {
                // Get ShowDetailPage and ViewModel from DI
                var detailPage = _serviceProvider.GetRequiredService<ShowDetailPage>();
                var detailViewModel = _serviceProvider.GetRequiredService<ShowDetailViewModel>();

                // Initialize the ViewModel with the show ID (it will fetch the details)
                await detailViewModel.InitializeAsync(item.ShowId);

                // Set binding context
                detailPage.BindingContext = detailViewModel;

                await Navigation.PushAsync(detailPage);
            }
        }
    }
}