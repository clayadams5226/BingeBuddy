using BingeBuddy.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace BingeBuddy.Models
{
    /// <summary>
    /// ViewModel wrapper for Episode to add UI-specific properties like IsWatched and air date calculations
    /// </summary>
    public partial class EpisodeViewModel : ObservableObject
    {
        public Episode Episode { get; set; }

        [ObservableProperty]
        private bool isWatched;

        [ObservableProperty]
        private bool isNextToWatch;

        [ObservableProperty]
        private bool hasAired;

        public int EpisodeNumber => Episode?.EpisodeNumber ?? 0;
        public int SeasonNumber => Episode?.SeasonNumber ?? 0;
        public string EpisodeTitle => Episode?.Name ?? "Unknown Episode";
        public string EpisodeNumberText => $"E{EpisodeNumber:D2}";
        public string EpisodeDescription => Episode?.Overview ?? "No description available";
        public string StillUrl => Episode?.StillUrl ?? string.Empty;

        public string AirDateText
        {
            get
            {
                if (string.IsNullOrEmpty(Episode?.AirDate))
                    return "Air date TBA";

                if (DateTime.TryParse(Episode.AirDate, out DateTime airDate))
                {
                    if (airDate > DateTime.Now)
                        return $"Airs {airDate:MMM d, yyyy}";
                    else
                        return $"Aired {airDate:MMM d, yyyy}";
                }

                return Episode.AirDate;
            }
        }

        public string RuntimeText
        {
            get
            {
                if (Episode?.Runtime > 0)
                    return $"{Episode.Runtime}min";
                return string.Empty;
            }
        }

        public EpisodeViewModel(Episode episode)
        {
            Episode = episode;
            IsWatched = false;
            IsNextToWatch = false;

            // Check if episode has aired
            if (!string.IsNullOrEmpty(episode?.AirDate) && DateTime.TryParse(episode.AirDate, out DateTime airDate))
            {
                HasAired = airDate <= DateTime.Now;
            }
            else
            {
                HasAired = true; // Assume aired if no date
            }
        }
    }
}
