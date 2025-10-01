using BingeBuddy.Helpers;
using BingeBuddy.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingeBuddy.Services
{
    public class TMDbApiService : ITVShowApiService
    {
        private readonly HttpClient _httpClient;
        private const string API_KEY = Constants.TMDB_API_KEY;

        public TMDbApiService()
        {
            var handler = new HttpClientHandler();

#if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.themoviedb.org/3/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<List<Show>> SearchShowsAsync(string query)
        {
            try
            {
                var url = $"search/tv?api_key={API_KEY}&query={Uri.EscapeDataString(query)}";
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Calling: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var shows = result["results"]?.ToObject<List<Show>>() ?? new List<Show>();
                    System.Diagnostics.Debug.WriteLine($"[TMDb API] Found {shows.Count} shows");
                    return shows;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[TMDb API] Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Exception: {ex.Message}");
            }

            return new List<Show>();
        }

        public async Task<Show> GetShowDetailsAsync(int showId)
        {
            try
            {
                var url = $"tv/{showId}?api_key={API_KEY}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var show = JsonConvert.DeserializeObject<Show>(json);
                    return show;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Error getting show details: {ex.Message}");
            }

            return null;
        }

        public async Task<List<Episode>> GetSeasonEpisodesAsync(int showId, int seasonNumber)
        {
            try
            {
                var url = $"tv/{showId}/season/{seasonNumber}?api_key={API_KEY}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var episodes = result["episodes"]?.ToObject<List<Episode>>() ?? new List<Episode>();
                    return episodes;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Error getting episodes: {ex.Message}");
            }

            return new List<Episode>();
        }

        public async Task<List<Show>> GetTrendingShowsAsync()
        {
            try
            {
                var url = $"trending/tv/day?api_key={API_KEY}";
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Calling: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[TMDb API] Response length: {json.Length}");

                    var result = JObject.Parse(json);
                    var shows = result["results"]?.ToObject<List<Show>>() ?? new List<Show>();

                    System.Diagnostics.Debug.WriteLine($"[TMDb API] Parsed {shows.Count} trending shows");
                    return shows;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[TMDb API] Error Response: {errorContent.Substring(0, Math.Min(200, errorContent.Length))}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Stack: {ex.StackTrace}");
            }

            return new List<Show>();
        }

        public async Task<List<Show>> GetPopularShowsAsync()
        {
            try
            {
                var url = $"tv/popular?api_key={API_KEY}&page=1";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var shows = result["results"]?.ToObject<List<Show>>() ?? new List<Show>();
                    return shows;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TMDb API] Error getting popular shows: {ex.Message}");
            }

            return new List<Show>();
        }
    }
}