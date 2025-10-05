using BingeBuddy.Helpers;
using BingeBuddy.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddy.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public DatabaseService()
        {
        }

        private async Task InitAsync()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<UserShow>();
            await _database.CreateTableAsync<WatchedEpisode>();
        }

        // User Shows
        public async Task<List<UserShow>> GetUserShowsAsync()
        {
            await InitAsync();
            return await _database.Table<UserShow>().ToListAsync();
        }

        public async Task<UserShow> GetUserShowAsync(int showId)
        {
            await InitAsync();
            return await _database.Table<UserShow>()
                .Where(s => s.ShowId == showId)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserShowAsync(UserShow show)
        {
            await InitAsync();
            if (show.Id != 0)
            {
                return await _database.UpdateAsync(show);
            }
            else
            {
                return await _database.InsertAsync(show);
            }
        }

        public async Task<int> DeleteUserShowAsync(UserShow show)
        {
            await InitAsync();
            return await _database.DeleteAsync(show);
        }

        // Watched Episodes
        public async Task<List<WatchedEpisode>> GetWatchedEpisodesAsync(int showId)
        {
            await InitAsync();
            return await _database.Table<WatchedEpisode>()
                .Where(e => e.ShowId == showId)
                .ToListAsync();
        }

        public async Task<bool> IsEpisodeWatchedAsync(int showId, int seasonNumber, int episodeNumber)
        {
            await InitAsync();
            var episode = await _database.Table<WatchedEpisode>()
                .Where(e => e.ShowId == showId &&
                           e.SeasonNumber == seasonNumber &&
                           e.EpisodeNumber == episodeNumber)
                .FirstOrDefaultAsync();

            return episode != null;
        }

        public async Task<int> MarkEpisodeWatchedAsync(int showId, int seasonNumber, int episodeNumber)
        {
            await InitAsync();

            // Check if already marked
            var existing = await _database.Table<WatchedEpisode>()
                .Where(e => e.ShowId == showId &&
                           e.SeasonNumber == seasonNumber &&
                           e.EpisodeNumber == episodeNumber)
                .FirstOrDefaultAsync();

            if (existing != null)
                return 0;

            var watchedEpisode = new WatchedEpisode
            {
                ShowId = showId,
                SeasonNumber = seasonNumber,
                EpisodeNumber = episodeNumber,
                WatchedDate = DateTime.Now
            };

            return await _database.InsertAsync(watchedEpisode);
        }

        public async Task<int> UnmarkEpisodeWatchedAsync(int showId, int seasonNumber, int episodeNumber)
        {
            await InitAsync();

            var episode = await _database.Table<WatchedEpisode>()
                .Where(e => e.ShowId == showId &&
                           e.SeasonNumber == seasonNumber &&
                           e.EpisodeNumber == episodeNumber)
                .FirstOrDefaultAsync();

            if (episode != null)
            {
                return await _database.DeleteAsync(episode);
            }

            return 0;
        }

        // Progress Calculation Methods
        public async Task<ShowProgress> GetShowProgressAsync(int showId, List<Season> seasons)
        {
            await InitAsync();

            var progress = new ShowProgress();
            var watchedEpisodes = await GetWatchedEpisodesAsync(showId);
            var watchedSet = new HashSet<(int season, int episode)>(
                watchedEpisodes.Select(e => (e.SeasonNumber, e.EpisodeNumber))
            );

            // Calculate totals (exclude season 0 - specials)
            var allEpisodes = seasons
                .Where(s => s.SeasonNumber > 0)
                .SelectMany(s => s.Episodes ?? new List<Episode>())
                .OrderBy(e => e.SeasonNumber)
                .ThenBy(e => e.EpisodeNumber)
                .ToList();

            progress.TotalEpisodes = allEpisodes.Count;
            progress.WatchedEpisodes = watchedSet.Count;

            // Find last watched episode and next to watch
            EpisodeViewModel nextEpisode = null;
            int lastWatchedSeason = 0;
            int lastWatchedEpisode = 0;

            foreach (var episode in allEpisodes)
            {
                bool isWatched = watchedSet.Contains((episode.SeasonNumber, episode.EpisodeNumber));

                if (isWatched)
                {
                    lastWatchedSeason = episode.SeasonNumber;
                    lastWatchedEpisode = episode.EpisodeNumber;
                }
                else if (nextEpisode == null)
                {
                    // First unwatched episode is the next to watch
                    nextEpisode = new EpisodeViewModel(episode)
                    {
                        IsWatched = false,
                        IsNextToWatch = true
                    };
                }
            }

            // Set current position (last watched or first episode if none watched)
            if (lastWatchedSeason > 0)
            {
                progress.CurrentSeason = lastWatchedSeason;
                progress.CurrentEpisode = lastWatchedEpisode;
            }
            else if (allEpisodes.Any())
            {
                progress.CurrentSeason = allEpisodes.First().SeasonNumber;
                progress.CurrentEpisode = 0;
            }

            progress.NextEpisode = nextEpisode;

            // Update UserShow with last watched info
            var userShow = await GetUserShowAsync(showId);
            if (userShow != null && (userShow.LastWatchedSeason != lastWatchedSeason || userShow.LastWatchedEpisode != lastWatchedEpisode))
            {
                userShow.LastWatchedSeason = lastWatchedSeason;
                userShow.LastWatchedEpisode = lastWatchedEpisode;

                // Update status based on progress
                if (progress.IsCompleted)
                    userShow.Status = "Completed";
                else if (progress.IsStarted)
                    userShow.Status = "Watching";

                await SaveUserShowAsync(userShow);
            }

            return progress;
        }

        public async Task<int> MarkSeasonWatchedAsync(int showId, int seasonNumber, List<Episode> episodes)
        {
            await InitAsync();

            int count = 0;
            foreach (var episode in episodes)
            {
                var result = await MarkEpisodeWatchedAsync(showId, seasonNumber, episode.EpisodeNumber);
                if (result > 0)
                    count++;
            }

            return count;
        }

        public async Task<int> UnmarkSeasonWatchedAsync(int showId, int seasonNumber)
        {
            await InitAsync();

            var episodesToDelete = await _database.Table<WatchedEpisode>()
                .Where(e => e.ShowId == showId && e.SeasonNumber == seasonNumber)
                .ToListAsync();

            int count = 0;
            foreach (var episode in episodesToDelete)
            {
                await _database.DeleteAsync(episode);
                count++;
            }

            return count;
        }

        public async Task<Dictionary<(int season, int episode), bool>> GetWatchedEpisodeDictionaryAsync(int showId)
        {
            await InitAsync();

            var watchedEpisodes = await GetWatchedEpisodesAsync(showId);
            return watchedEpisodes.ToDictionary(
                e => (e.SeasonNumber, e.EpisodeNumber),
                e => true
            );
        }
    }
}