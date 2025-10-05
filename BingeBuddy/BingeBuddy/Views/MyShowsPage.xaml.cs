using BingeBuddy.Models;
using BingeBuddy.ViewModels;

namespace BingeBuddy.Views
{
    public partial class MyShowsPage : ContentPage
    {
        private readonly MyShowsViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;

        public MyShowsPage(MyShowsViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _serviceProvider = serviceProvider;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadShowsCommand.ExecuteAsync(null);
        }

        private async void OnShowTapped(object sender, EventArgs e)
        {
            if (sender is VerticalStackLayout layout && layout.BindingContext is UserShow userShow)
            {
                // Get ShowDetailPage and ViewModel from DI
                var detailPage = _serviceProvider.GetRequiredService<ShowDetailPage>();
                var detailViewModel = _serviceProvider.GetRequiredService<ShowDetailViewModel>();

                // Initialize with the show ID (will fetch full details)
                await detailViewModel.InitializeAsync(userShow.ShowId);

                // Set binding context
                detailPage.BindingContext = detailViewModel;

                await Navigation.PushAsync(detailPage);
            }
        }
    }
}