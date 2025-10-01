using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BingeBuddy.Models
{
    public class Show
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("first_air_date")]
        public string FirstAirDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("number_of_seasons")]
        public int NumberOfSeasons { get; set; }

        [JsonProperty("number_of_episodes")]
        public int NumberOfEpisodes { get; set; }

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; }

        [JsonProperty("networks")]
        public List<Network> Networks { get; set; }

        [JsonProperty("seasons")]
        public List<Season> Seasons { get; set; }

        // Helper properties
        public string PosterUrl => !string.IsNullOrEmpty(PosterPath)
            ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
            : "https://via.placeholder.com/500x750/2A2A2A/888888?text=No+Image";

        public string BackdropUrl => !string.IsNullOrEmpty(BackdropPath)
            ? $"https://image.tmdb.org/t/p/original{BackdropPath}"
            : "https://via.placeholder.com/1920x1080/2A2A2A/888888?text=No+Image";

        public string Year => !string.IsNullOrEmpty(FirstAirDate) && FirstAirDate.Length >= 4
            ? FirstAirDate.Substring(0, 4)
            : "N/A";
    }

    public class Season
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }

        [JsonProperty("episode_count")]
        public int EpisodeCount { get; set; }

        [JsonProperty("air_date")]
        public string AirDate { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("episodes")]
        public List<Episode> Episodes { get; set; }
    }

    public class Episode
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }

        [JsonProperty("episode_number")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("air_date")]
        public string AirDate { get; set; }

        [JsonProperty("runtime")]
        public int Runtime { get; set; }

        [JsonProperty("still_path")]
        public string StillPath { get; set; }

        public string StillUrl => !string.IsNullOrEmpty(StillPath)
            ? $"https://image.tmdb.org/t/p/w300{StillPath}"
            : "https://via.placeholder.com/300x169/2A2A2A/888888?text=No+Image";
    }

    public class Network
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logo_path")]
        public string LogoPath { get; set; }
    }

    public class Genre
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}