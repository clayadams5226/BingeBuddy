using BingeBuddy.ViewModels;

namespace BingeBuddy.Views
{
    public partial class WatchlistPage : ContentPage
    {
        private readonly WatchlistViewModel _viewModel;

        public WatchlistPage(WatchlistViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadWatchlistCommand.ExecuteAsync(null);
        }
    }
}