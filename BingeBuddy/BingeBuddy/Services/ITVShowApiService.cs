using BingeBuddy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddy.Services
{
    public interface ITVShowApiService
    {
        Task<List<Show>> SearchShowsAsync(string query);
        Task<Show> GetShowDetailsAsync(int showId);
        Task<List<Episode>> GetSeasonEpisodesAsync(int showId, int seasonNumber);
        Task<List<Show>> GetTrendingShowsAsync();
        Task<List<Show>> GetPopularShowsAsync();
        Task<List<Show>> GetSimilarShowsAsync(int showId);


    }


}