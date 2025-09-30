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
        private const string BASE_URL = Constants.TMDB_BASE_URL;

        public TMDbApiService()
        {
            var handler = new HttpClientHandler();

            // For development/testing - bypass SSL validation
            // Remove this in production!
#if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BASE_URL),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<List<Show>> SearchShowsAsync(string query)
        {
            try
            {
                var url = $"/search/tv?api_key={API_KEY}&query={Uri.EscapeDataString(query)}&page=1";
                System.Diagnostics.Debug.WriteLine($"Calling URL: {BASE_URL}{url}");

                var response = await _httpClient.GetAsync(url);

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Response JSON: {json.Substring(0, Math.Min(200, json.Length))}...");

                    var result = JObject.Parse(json);
                    var shows = result["results"].ToObject<List<Show>>();
                    return shows ?? new List<Show>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error Response: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching shows: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return new List<Show>();
        }

        public async Task<Show> GetShowDetailsAsync(int showId)
        {
            try
            {
                var url = $"/tv/{showId}?api_key={API_KEY}&append_to_response=seasons,credits";
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
                System.Diagnostics.Debug.WriteLine($"Error getting show details: {ex.Message}");
            }

            return null;
        }

        public async Task<List<Episode>> GetSeasonEpisodesAsync(int showId, int seasonNumber)
        {
            try
            {
                var url = $"/tv/{showId}/season/{seasonNumber}?api_key={API_KEY}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var episodes = result["episodes"].ToObject<List<Episode>>();
                    return episodes ?? new List<Episode>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting episodes: {ex.Message}");
            }

            return new List<Episode>();
        }

        public async Task<List<Show>> GetTrendingShowsAsync()
        {
            try
            {
                var url = $"/trending/tv/week?api_key={API_KEY}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var shows = result["results"].ToObject<List<Show>>();
                    return shows ?? new List<Show>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting trending shows: {ex.Message}");
            }

            return new List<Show>();
        }

        public async Task<List<Show>> GetPopularShowsAsync()
        {
            try
            {
                var url = $"/tv/popular?api_key={API_KEY}&page=1";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var shows = result["results"].ToObject<List<Show>>();
                    return shows ?? new List<Show>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting popular shows: {ex.Message}");
            }

            return new List<Show>();
        }
    }
}