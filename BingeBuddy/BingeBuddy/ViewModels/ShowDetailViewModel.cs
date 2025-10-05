using BingeBuddy.Models;
using BingeBuddy.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BingeBuddy.ViewModels
{
    public partial class ShowDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ITVShowApiService _apiService;

        [ObservableProperty]
        private Show currentShow;

        [ObservableProperty]
        private UserShow userShowData;

        [ObservableProperty]
        private ObservableCollection<Show> similarShows;

        [ObservableProperty]
        private ObservableCollection<SeasonViewModel> seasons;

        [ObservableProperty]
        private ShowProgress progress;

        [ObservableProperty]
        private bool isInLibrary;

        [ObservableProperty]
        private bool isLoadingSeasons;

        [ObservableProperty]
        private bool isLoadingSimilar;

        [ObservableProperty]
        private bool hasSeasons;

        [ObservableProperty]
        private string addButtonText;

        [ObservableProperty]
        private Color addButtonColor;

        public ShowDetailViewModel(DatabaseService databaseService, ITVShowApiService apiService)
        {
            _databaseService = databaseService;
            _apiService = apiService;

            SimilarShows = new ObservableCollection<Show>();
            Seasons = new ObservableCollection<SeasonViewModel>();
            Progress = new ShowProgress();
            AddButtonText = "Add to My Shows";
            AddButtonColor = Color.FromArgb("#3B82F6");
        }

        public async Task InitializeAsync(Show show)
        {
            CurrentShow = show;
            Title = show.Name;

            await Task.WhenAll(
                LoadShowDetailsAsync(),
                LoadSimilarShowsAsync(),
                CheckIfInLibraryAsync()
            );
        }

        public async Task InitializeAsync(int showId)
        {
            try
            {
                IsBusy = true;
                var show = await _apiService.GetShowDetailsAsync(showId);
                if (show != null)
                {
                    await InitializeAsync(show);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ShowDetailVM] Error initializing with showId: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadShowDetailsAsync()
        {
            if (CurrentShow == null) return;

            try
            {
                IsLoadingSeasons = true;

                // Get full show details with seasons
                var fullShow = await _apiService.GetShowDetailsAsync(CurrentShow.Id);
                if (fullShow != null)
                {
                    CurrentShow = fullShow;
                }

                // Load seasons and episodes
                if (CurrentShow.Seasons != null && CurrentShow.Seasons.Any())
                {
                    Seasons.Clear();

                    // Get watched episodes dictionary for performance
                    var watchedDict = await _databaseService.GetWatchedEpisodeDictionaryAsync(CurrentShow.Id);

                    // Load episodes for all seasons in parallel (excluding season 0 - specials)
                    var seasonsToLoad = CurrentShow.Seasons.Where(s => s.SeasonNumber > 0).OrderBy(s => s.SeasonNumber).ToList();

                    // Create parallel tasks to fetch episodes for all seasons
                    var episodeTasks = seasonsToLoad.Select(season =>
                        _apiService.GetSeasonEpisodesAsync(CurrentShow.Id, season.SeasonNumber)
                    ).ToList();

                    // Wait for all episode fetches to complete in parallel
                    var allEpisodes = await Task.WhenAll(episodeTasks);

                    // Process the results
                    for (int i = 0; i < seasonsToLoad.Count; i++)
                    {
                        var season = seasonsToLoad[i];
                        var episodes = allEpisodes[i];
                        season.Episodes = episodes; // Update the season with episodes

                        var seasonViewModel = new SeasonViewModel(season);

                        foreach (var episode in episodes.OrderBy(e => e.EpisodeNumber))
                        {
                            var episodeViewModel = new EpisodeViewModel(episode)
                            {
                                IsWatched = watchedDict.ContainsKey((season.SeasonNumber, episode.EpisodeNumber))
                            };
                            seasonViewModel.Episodes.Add(episodeViewModel);
                        }

                        seasonViewModel.UpdateProgress();
                        Seasons.Add(seasonViewModel);
                    }

                    HasSeasons = Seasons.Any();

                    // Calculate overall progress
                    Progress = await _databaseService.GetShowProgressAsync(CurrentShow.Id, CurrentShow.Seasons);

                    // Mark next episode
                    if (Progress.HasNextEpisode)
                    {
                        var nextEp = Progress.NextEpisode;
                        var seasonVm = Seasons.FirstOrDefault(s => s.SeasonNumber == nextEp.SeasonNumber);
                        if (seasonVm != null)
                        {
                            var episodeVm = seasonVm.Episodes.FirstOrDefault(e => e.EpisodeNumber == nextEp.EpisodeNumber);
                            if (episodeVm != null)
                            {
                                episodeVm.IsNextToWatch = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ShowDetailVM] Error loading show details: {ex.Message}");
                Debug.WriteLine($"[ShowDetailVM] Stack: {ex.StackTrace}");
            }
            finally
            {
                IsLoadingSeasons = false;
            }
        }

        [RelayCommand]
        private async Task LoadSimilarShowsAsync()
        {
            if (CurrentShow == null) return;

            try
            {
                IsLoadingSimilar = true;
                var shows = await _apiService.GetSimilarShowsAsync(CurrentShow.Id);

                SimilarShows.Clear();
                if (shows != null)
                {
                    foreach (var show in shows.Take(10))
                    {
                        SimilarShows.Add(show);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ShowDetailVM] Error loading similar shows: {ex.Message}");
            }
            finally
            {
                IsLoadingSimilar = false;
            }
        }

        [RelayCommand]
        private async Task CheckIfInLibraryAsync()
        {
            if (CurrentShow == null) return;

            try
            {
                UserShowData = await _databaseService.GetUserShowAsync(CurrentShow.Id);
                IsInLibrary = UserShowData != null;

                if (IsInLibrary)
                {
                    AddButtonText = "In Library ✓";
                    AddButtonColor = Color.FromArgb("#4B5563");
                }
                else
                {
                    AddButtonText = "Add to My Shows";
                    AddButtonColor = Color.FromArgb("#3B82F6");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ShowDetailVM] Error checking library status: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddToMyShowsAsync()
        {
            if (CurrentShow == null || IsInLibrary) return;

            try
            {
                var userShow = new UserShow
                {
                    ShowId = CurrentShow.Id,
                    Title = CurrentShow.Name,
                    PosterUrl = CurrentShow.PosterUrl,
                    Status = "Watching",
                    DateAdded = DateTime.Now,
                    LastWatchedSeason = 0,
                    LastWatchedEpisode = 0
                };

                await _databaseService.SaveUserShowAsync(userShow);
                UserShowData = userShow;
                IsInLibrary = true;
                AddButtonText = "In Library ✓";
                AddButtonColor = Color.FromArgb("#4B5563");

                Debug.WriteLine($"[ShowDetailVM] Added {CurrentShow.Name} to library");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ShowDetailVM] Error adding show: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ToggleEpisodeWatchedAsync(EpisodeViewModel episode)
        {
            if (episode == null || CurrentShow == null) return;

            try
            {
                // Ensure show is in library before tracking episodes
                if (!IsInLibrary)
                {
                    await AddToMyShowsAsync();
                }

                if (episode.IsWatched)
                {
                    // Unmark as watched
                    await _databaseService.UnmarkEpisodeWatchedAsync(
                        CurrentShow.Id,
                        episode.SeasonNumber,
                        episode.EpisodeNumber
                    );
                    episode.IsWatched = false;
                    Debug.WriteLine($"[ShowDetailVM] Unmarked S{episode.SeasonNumber}E{episode.EpisodeNumber} as watched");
                }
                else
                {
                    // Mark as watched
                    await _databaseService.MarkEpisodeWatchedAsync(
                        CurrentShow.Id,
                        episode.SeasonNumber,
                        episode.EpisodeNumber
                    );
                    episode.IsWatched = true;
                    Debug.WriteLine($"[ShowDetailVM] Marked S{episode.SeasonNumber}E{episode.EpisodeNumber} as watched");
                }

                // Update season progress
                var season = Seasons.FirstOrDefault(s => s.SeasonNumber == episode.SeasonNumber);
                if (season != null)
                {
                    season.UpdateProgress();
                }

                // Recalculate overall progress
                Progress = await _databaseService.GetShowProgressAsync(CurrentShow.Id, CurrentShow.Seasons);

                // Update next episode marker
                UpdateNextEpisodeMarker();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ShowDetailVM] Error toggling episode: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ToggleSeasonWatchedAsync(SeasonViewModel season)
        {
            if (season == null || CurrentShow == null) return;

            try
            {
                // Ensure show is in library
                if (!IsInLibrary)
                {
                    await AddToMyShowsAsync();
                }

                bool markAsWatched = season.WatchedCount < season.TotalEpisodes;

                if (markAsWatched)
                {
                    // Mark all episodes as watched
                    await _databaseService.MarkSeasonWatchedAsync(
                        CurrentShow.Id,
                        season.SeasonNumber,
                        season.Season.Episodes
                    );

                    foreach (var episode in season.Episodes)
                    {
                        episode.IsWatched = true;
                    }
                }
                else
                {
                    // Unmark all episodes
                    await _databaseService.UnmarkSeasonWatchedAsync(
                        CurrentShow.Id,
                        season.SeasonNumber
                    );

                    foreach (var episode in season.Episodes)
                    {
                        episode.IsWatched = false;
                    }
                }

                season.UpdateProgress();

                // Recalculate overall progress
                Progress = await _databaseService.GetShowProgressAsync(CurrentShow.Id, CurrentShow.Seasons);

                // Update next episode marker
                UpdateNextEpisodeMarker();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ShowDetailVM] Error toggling season: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ToggleSeasonExpanded(SeasonViewModel season)
        {
            if (season != null)
            {
                season.IsExpanded = !season.IsExpanded;
            }
        }

        private void UpdateNextEpisodeMarker()
        {
            // Clear all next-to-watch markers
            foreach (var season in Seasons)
            {
                foreach (var episode in season.Episodes)
                {
                    episode.IsNextToWatch = false;
                }
            }

            // Set the new next episode
            if (Progress.HasNextEpisode)
            {
                var nextEp = Progress.NextEpisode;
                var seasonVm = Seasons.FirstOrDefault(s => s.SeasonNumber == nextEp.SeasonNumber);
                if (seasonVm != null)
                {
                    var episodeVm = seasonVm.Episodes.FirstOrDefault(e => e.EpisodeNumber == nextEp.EpisodeNumber);
                    if (episodeVm != null)
                    {
                        episodeVm.IsNextToWatch = true;
                    }
                }
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await Task.WhenAll(
                LoadShowDetailsAsync(),
                LoadSimilarShowsAsync(),
                CheckIfInLibraryAsync()
            );
            IsRefreshing = false;
        }
    }
}
