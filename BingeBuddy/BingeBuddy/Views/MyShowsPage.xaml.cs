using BingeBuddy.ViewModels;

namespace BingeBuddy.Views
{
    public partial class MyShowsPage : ContentPage
    {
        private readonly MyShowsViewModel _viewModel;

        public MyShowsPage(MyShowsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadShowsCommand.ExecuteAsync(null);
        }
    }
}