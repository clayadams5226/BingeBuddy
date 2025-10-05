using BingeBuddy.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BingeBuddy.Models
{
    /// <summary>
    /// ViewModel wrapper for Season to add UI-specific properties like IsExpanded and watched episode tracking
    /// </summary>
    public partial class SeasonViewModel : ObservableObject
    {
        public Season Season { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        [ObservableProperty]
        private ObservableCollection<EpisodeViewModel> episodes;

        [ObservableProperty]
        private int watchedCount;

        [ObservableProperty]
        private int totalEpisodes;

        [ObservableProperty]
        private double progressPercentage;

        public int SeasonNumber => Season?.SeasonNumber ?? 0;
        public string SeasonName => Season?.Name ?? "Unknown Season";
        public string EpisodeCountText => $"{WatchedCount}/{TotalEpisodes} episodes";
        public string ProgressText => $"{ProgressPercentage:F0}%";

        public SeasonViewModel(Season season)
        {
            Season = season;
            Episodes = new ObservableCollection<EpisodeViewModel>();
            IsExpanded = false;
        }

        public void UpdateProgress()
        {
            WatchedCount = Episodes?.Count(e => e.IsWatched) ?? 0;
            TotalEpisodes = Episodes?.Count ?? 0;
            ProgressPercentage = TotalEpisodes > 0 ? (WatchedCount * 100.0 / TotalEpisodes) : 0;
        }
    }
}
