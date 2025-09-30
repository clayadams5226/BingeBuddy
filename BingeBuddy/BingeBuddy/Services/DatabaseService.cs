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
    }
}