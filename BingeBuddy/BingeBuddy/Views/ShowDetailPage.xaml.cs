using BingeBuddy.Models;
using BingeBuddy.ViewModels;

namespace BingeBuddy.Views
{
    public partial class ShowDetailPage : ContentPage
    {
        private readonly ShowDetailViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;

        public ShowDetailPage(ShowDetailViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _serviceProvider = serviceProvider;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // ViewModel will be initialized before navigation with the show data
            // So we just need to trigger loading if not already loaded
            if (_viewModel.CurrentShow == null)
            {
                // This shouldn't happen in normal flow, but handle it gracefully
                await Navigation.PopAsync();
            }
        }

        private async void OnSimilarShowTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is Show show)
            {
                // Get ShowDetailPage and ViewModel from DI
                var detailPage = _serviceProvider.GetRequiredService<ShowDetailPage>();
                var detailViewModel = _serviceProvider.GetRequiredService<ShowDetailViewModel>();

                // Initialize the ViewModel with the show data
                await detailViewModel.InitializeAsync(show);

                // Set binding context (already set in page constructor, but ensure it's the right instance)
                detailPage.BindingContext = detailViewModel;

                await Navigation.PushAsync(detailPage);
            }
        }
    }
}
