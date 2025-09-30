using SQLite;
using System;

namespace BingeBuddy.Models
{
    [Table("user_shows")]
    public class UserShow
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ShowId { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public string Status { get; set; } // "Watching", "Completed", "Dropped"
        public DateTime DateAdded { get; set; }
        public int LastWatchedSeason { get; set; }
        public int LastWatchedEpisode { get; set; }
    }

    [Table("watched_episodes")]
    public class WatchedEpisode
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ShowId { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime WatchedDate { get; set; }
    }
}