using BingeBuddy.Models;
using BingeBuddy.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BingeBuddy.ViewModels
{
    public partial class GlobalSearchViewModel : BaseViewModel
    {
        private readonly ITVShowApiService _apiService;
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private string searchQuery;

        [ObservableProperty]
        private ObservableCollection<ShowSearchResult> searchResults;

        [ObservableProperty]
        private bool hasResults;

        public GlobalSearchViewModel(ITVShowApiService apiService, DatabaseService databaseService)
        {
            _apiService = apiService;
            _databaseService = databaseService;
            SearchResults = new ObservableCollection<ShowSearchResult>();
        }

        [RelayCommand]
        private async Task SearchShows(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                SearchResults.Clear();
                HasResults = false;
                return;
            }

            try
            {
                IsBusy = true;
                SearchQuery = query;

                // Search API
                var shows = await _apiService.SearchShowsAsync(query);

                // Get user's saved shows to check which are already added
                var userShows = await _databaseService.GetUserShowsAsync();
                var userShowIds = userShows.Select(s => s.ShowId).ToHashSet();

                // Create search results with "already added" flag
                var results = shows.Select(show => new ShowSearchResult
                {
                    Show = show,
                    IsAlreadyAdded = userShowIds.Contains(show.Id)
                }).ToList();

                SearchResults.Clear();
                foreach (var result in results)
                {
                    SearchResults.Add(result);
                }

                HasResults = SearchResults.Any();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching shows: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task QuickAddShow(ShowSearchResult result)
        {
            try
            {
                if (result.IsAlreadyAdded)
                    return;

                var userShow = new UserShow
                {
                    ShowId = result.Show.Id,
                    Title = result.Show.Name,
                    PosterUrl = result.Show.PosterUrl,
                    Status = "Watching",
                    DateAdded = DateTime.Now,
                    LastWatchedSeason = 0,
                    LastWatchedEpisode = 0
                };

                await _databaseService.SaveUserShowAsync(userShow);

                // Update the result to show it's now added
                result.IsAlreadyAdded = true;

                Debug.WriteLine($"Added {result.Show.Name} to My Shows");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding show: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchQuery = string.Empty;
            SearchResults.Clear();
            HasResults = false;
        }
    }

    // Helper class for search results
    public partial class ShowSearchResult : ObservableObject
    {
        public Show Show { get; set; }

        [ObservableProperty]
        private bool isAlreadyAdded;
    }
}