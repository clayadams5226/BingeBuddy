using System;
using System.Net.Http;
using Microsoft.Maui.Controls;

namespace BingeBuddy
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            try
            {
                // Simple HTTP test first
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var testUrl = "https://api.themoviedb.org/3/search/tv?api_key=32abc095da76ccc08b0d657136d45b04&query=Breaking+Bad";

                System.Diagnostics.Debug.WriteLine($"Testing URL: {testUrl}");

                var response = await httpClient.GetAsync(testUrl);
                var content = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Content Length: {content.Length}");

                if (response.IsSuccessStatusCode && content.Length > 0)
                {
                    await DisplayAlert("API Test Success!",
                        $"Status: {response.StatusCode}\n\nFirst 200 chars:\n{content.Substring(0, Math.Min(200, content.Length))}",
                        "OK");
                }
                else
                {
                    await DisplayAlert("API Test Failed",
                        $"Status: {response.StatusCode}\n\nResponse: {content}",
                        "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                await DisplayAlert("Network Error",
                    $"Cannot reach the API.\n\nError: {ex.Message}\n\nCheck:\n1. Emulator has internet\n2. API key is correct",
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                await DisplayAlert("Error",
                    $"Exception: {ex.Message}\n\nType: {ex.GetType().Name}",
                    "OK");
            }
        }
    }
}