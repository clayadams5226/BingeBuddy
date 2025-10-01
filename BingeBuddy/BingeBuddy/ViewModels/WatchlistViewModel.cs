using BingeBuddy.Models;
using BingeBuddy.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BingeBuddy.ViewModels
{
    public partial class WatchlistViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ITVShowApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<WatchlistItem> upcomingEpisodes;

        [ObservableProperty]
        private bool hasEpisodes;

        public WatchlistViewModel(DatabaseService databaseService, ITVShowApiService apiService)
        {
            _databaseService = databaseService;
            _apiService = apiService;
            Title = "Watchlist";
            UpcomingEpisodes = new ObservableCollection<WatchlistItem>();
        }

        [RelayCommand]
        private async Task LoadWatchlist()
        {
            try
            {
                IsBusy = true;

                // Get user's shows
                var userShows = await _databaseService.GetUserShowsAsync();

                UpcomingEpisodes.Clear();

                // For now, just show placeholder data
                // In Phase 5, we'll calculate actual next episodes
                foreach (var show in userShows.Take(5))
                {
                    UpcomingEpisodes.Add(new WatchlistItem
                    {
                        ShowId = show.ShowId,
                        ShowTitle = show.Title,
                        PosterUrl = show.PosterUrl,
                        NextEpisode = $"S{show.LastWatchedSeason + 1}E{show.LastWatchedEpisode + 1}",
                        AirDate = "Coming soon"
                    });
                }

                HasEpisodes = UpcomingEpisodes.Any();

                Debug.WriteLine($"Loaded {UpcomingEpisodes.Count} watchlist items");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading watchlist: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadWatchlist();
            IsRefreshing = false;
        }
    }

    // Helper class for watchlist display
    public class WatchlistItem
    {
        public int ShowId { get; set; }
        public string ShowTitle { get; set; }
        public string PosterUrl { get; set; }
        public string NextEpisode { get; set; }
        public string AirDate { get; set; }
    }
}