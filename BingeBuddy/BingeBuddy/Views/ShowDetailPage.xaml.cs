using BingeBuddy.Models;
using BingeBuddy.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace BingeBuddy.Views
{
    public partial class ShowDetailPage : ContentPage
    {
        private readonly Show _show;
        private readonly DatabaseService _databaseService;
        private readonly ITVShowApiService _apiService;

        public Show CurrentShow { get; set; }
        public ObservableCollection<Show> SimilarShows { get; set; }
        public ICommand NavigateToShowCommand { get; }

        public ShowDetailPage(Show show, DatabaseService databaseService, ITVShowApiService apiService)
        {
            InitializeComponent();
            _show = show;
            CurrentShow = show;
            _databaseService = databaseService;
            _apiService = apiService;

            SimilarShows = new ObservableCollection<Show>();
            NavigateToShowCommand = new AsyncRelayCommand<Show>(NavigateToShowAsync);

            BindingContext = this;

            Title = show.Name;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Load similar shows when page appears
            if (SimilarShows.Count == 0)
            {
                LoadSimilarShows();
            }
        }

        private async Task NavigateToShowAsync(Show show)
        {
            if (show != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ShowDetail] Navigating to: {show.Name}");
                var detailPage = new ShowDetailPage(show, _databaseService, _apiService);
                await Navigation.PushAsync(detailPage);
            }
        }

        private async void LoadSimilarShows()
        {
            if (_apiService == null)
            {
                System.Diagnostics.Debug.WriteLine("[ShowDetail] API Service is null!");
                return;
            }

            try
            {
                SimilarShowsLoader.IsRunning = true;
                SimilarShowsSection.IsVisible = true;

                System.Diagnostics.Debug.WriteLine($"[ShowDetail] Loading similar shows for {_show.Name} (ID: {_show.Id})");
                var shows = await _apiService.GetSimilarShowsAsync(_show.Id);
                System.Diagnostics.Debug.WriteLine($"[ShowDetail] Received {shows?.Count ?? 0} similar shows");

                SimilarShows.Clear();
                if (shows != null)
                {
                    foreach (var show in shows.Take(10))
                    {
                        SimilarShows.Add(show);
                        System.Diagnostics.Debug.WriteLine($"[ShowDetail] Added similar show: {show.Name}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[ShowDetail] SimilarShows collection now has {SimilarShows.Count} items");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ShowDetail] Error loading similar shows: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ShowDetail] Stack trace: {ex.StackTrace}");
                SimilarShowsSection.IsVisible = false;
            }
            finally
            {
                SimilarShowsLoader.IsRunning = false;
            }
        }

        private async void OnAddToMyShowsClicked(object sender, EventArgs e)
        {
            if (_databaseService == null)
            {
                await DisplayAlert("Error", "Database service not available", "OK");
                return;
            }

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

                AddButton.Text = "Added ✓";
                AddButton.BackgroundColor = Color.FromArgb("#4B5563");
                AddButton.IsEnabled = false;

                await DisplayAlert("Success", $"{_show.Name} added to My Shows!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not add show: {ex.Message}", "OK");
            }
        }
    }
}