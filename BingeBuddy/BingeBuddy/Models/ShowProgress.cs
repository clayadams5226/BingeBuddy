using System;

namespace BingeBuddy.Models
{
    /// <summary>
    /// Represents calculated progress information for a show
    /// </summary>
    public class ShowProgress
    {
        public int TotalEpisodes { get; set; }
        public int WatchedEpisodes { get; set; }
        public double CompletionPercentage => TotalEpisodes > 0 ? (WatchedEpisodes * 100.0 / TotalEpisodes) : 0;

        public int CurrentSeason { get; set; }
        public int CurrentEpisode { get; set; }
        public string ProgressText => $"S{CurrentSeason:D2}E{CurrentEpisode:D2}";

        public EpisodeViewModel NextEpisode { get; set; }
        public bool HasNextEpisode => NextEpisode != null;

        public bool IsCompleted => WatchedEpisodes > 0 && WatchedEpisodes == TotalEpisodes;
        public bool IsStarted => WatchedEpisodes > 0;

        public string StatusText
        {
            get
            {
                if (IsCompleted)
                    return "Completed";
                if (IsStarted)
                    return $"Watching - {ProgressText}";
                return "Not Started";
            }
        }
    }
}
