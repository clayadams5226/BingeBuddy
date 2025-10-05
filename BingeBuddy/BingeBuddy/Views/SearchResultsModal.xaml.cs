using BingeBuddy.ViewModels;
using BingeBuddy.Services;

namespace BingeBuddy.Views
{
    public partial class SearchResultsModal : ContentPage
    {
        private readonly GlobalSearchViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;

        public SearchResultsModal(GlobalSearchViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _serviceProvider = serviceProvider;
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
                // Get ShowDetailPage and ViewModel from DI
                var detailPage = _serviceProvider.GetRequiredService<ShowDetailPage>();
                var detailViewModel = _serviceProvider.GetRequiredService<ShowDetailViewModel>();

                // Initialize the ViewModel with the show data
                await detailViewModel.InitializeAsync(result.Show);

                // Set binding context (already set in page constructor, but ensure it's the right instance)
                detailPage.BindingContext = detailViewModel;

                await Navigation.PushAsync(detailPage);
            }
        }
    }
}