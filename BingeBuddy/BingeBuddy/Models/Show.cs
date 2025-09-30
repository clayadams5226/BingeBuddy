using System;
using System.Collections.Generic;

namespace BingeBuddy.Models
{
    public class Show
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
        public double VoteAverage { get; set; }
        public string FirstAirDate { get; set; }
        public string Status { get; set; }
        public int NumberOfSeasons { get; set; }
        public int NumberOfEpisodes { get; set; }
        public List<string> Genres { get; set; }
        public List<Network> Networks { get; set; }
        public List<Season> Seasons { get; set; }

        // Helper properties
        public string PosterUrl => !string.IsNullOrEmpty(PosterPath)
            ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
            : "placeholder.png";

        public string BackdropUrl => !string.IsNullOrEmpty(BackdropPath)
            ? $"https://image.tmdb.org/t/p/original{BackdropPath}"
            : "placeholder.png";

        public string Year => !string.IsNullOrEmpty(FirstAirDate)
            ? DateTime.Parse(FirstAirDate).Year.ToString()
            : "N/A";
    }

    public class Season
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeCount { get; set; }
        public string AirDate { get; set; }
        public string PosterPath { get; set; }
        public List<Episode> Episodes { get; set; }
    }

    public class Episode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string AirDate { get; set; }
        public int Runtime { get; set; }
        public string StillPath { get; set; }

        public string StillUrl => !string.IsNullOrEmpty(StillPath)
            ? $"https://image.tmdb.org/t/p/w300{StillPath}"
            : "placeholder.png";
    }

    public class Network
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}