using BingeBuddy.Models;
using BingeBuddy.Services;

namespace BingeBuddy.Views
{
    public partial class ShowDetailPage : ContentPage
    {
        private readonly Show _show;
        private readonly DatabaseService _databaseService;

        public ShowDetailPage(Show show)
        {
            InitializeComponent();
            _show = show;

            // Get DatabaseService from dependency injection
            _databaseService = Handler.MauiContext.Services.GetService<DatabaseService>();

            BindingContext = show;
        }

        private async void OnAddToMyShowsClicked(object sender, EventArgs e)
        {
            try
            {
                var userShow = new UserShow
                {
                    ShowId = _show.Id,
                    Title = _show.Name,
                    PosterUrl = _show.PosterUrl,
                    Status = "Watching",
                    DateAdded = DateTime.Now,
                    LastWatchedSeason = 0,
                    LastWatchedEpisode = 0
                };

                await _databaseService.SaveUserShowAsync(userShow);

                await DisplayAlert("Success", $"{_show.Name} added to My Shows!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not add show: {ex.Message}", "OK");
            }
        }
    }
}