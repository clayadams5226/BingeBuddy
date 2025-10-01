using BingeBuddy.ViewModels;
using BingeBuddy.Services;

namespace BingeBuddy.Views
{
    public partial class SearchResultsModal : ContentPage
    {
        private readonly GlobalSearchViewModel _viewModel;
        private readonly DatabaseService _databaseService;

        public SearchResultsModal(GlobalSearchViewModel viewModel, DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _databaseService = databaseService;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SearchInput.Focus();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            _viewModel.ClearSearchCommand.Execute(null);
            await Navigation.PopModalAsync();
        }

        private async void OnShowTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is ShowSearchResult result)
            {
                // Navigate to show detail page
                var detailPage = new ShowDetailPage(result.Show);
                await Navigation.PushAsync(detailPage);
            }
        }
    }
}