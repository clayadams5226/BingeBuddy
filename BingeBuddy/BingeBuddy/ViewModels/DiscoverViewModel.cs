using BingeBuddy.Models;
using BingeBuddy.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BingeBuddy.ViewModels
{
    public partial class DiscoverViewModel : BaseViewModel
    {
        private readonly ITVShowApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<Show> trendingShows;

        [ObservableProperty]
        private ObservableCollection<Show> popularShows;

        [ObservableProperty]
        private bool isLoadingTrending;

        public DiscoverViewModel(ITVShowApiService apiService)
        {
            _apiService = apiService;
            Title = "Discover";
            TrendingShows = new ObservableCollection<Show>();
            PopularShows = new ObservableCollection<Show>();
        }

        [RelayCommand]
        private async Task LoadTrendingShows()
        {
            if (IsLoadingTrending)
                return;

            try
            {
                IsLoadingTrending = true;

                Debug.WriteLine("[DiscoverVM] Starting to load trending shows...");
                var shows = await _apiService.GetTrendingShowsAsync();
                Debug.WriteLine($"[DiscoverVM] API returned {shows?.Count ?? 0} shows");

                TrendingShows.Clear();

                if (shows != null && shows.Any())
                {
                    foreach (var show in shows.Take(20))
                    {
                        Debug.WriteLine($"[DiscoverVM] Adding show: {show.Name}, PosterPath: {show.PosterPath}, PosterUrl: {show.PosterUrl}");
                        TrendingShows.Add(show);
                    }
                }

                Debug.WriteLine($"[DiscoverVM] TrendingShows collection now has {TrendingShows.Count} items");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DiscoverVM] Error loading trending shows: {ex.Message}");
                Debug.WriteLine($"[DiscoverVM] Stack: {ex.StackTrace}");
            }
            finally
            {
                IsLoadingTrending = false;
            }
        }

        [RelayCommand]
        private async Task LoadPopularShows()
        {
            try
            {
                var shows = await _apiService.GetPopularShowsAsync();

                PopularShows.Clear();
                foreach (var show in shows.Take(20))
                {
                    PopularShows.Add(show);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading popular shows: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadTrendingShows();
            await LoadPopularShows();
            IsRefreshing = false;
        }
    }
}