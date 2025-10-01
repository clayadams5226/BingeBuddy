using BingeBuddy.Models;
using BingeBuddy.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BingeBuddy.ViewModels
{
    public partial class MyShowsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ITVShowApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<UserShow> myShows;

        [ObservableProperty]
        private bool hasShows;

        [ObservableProperty]
        private int showCount;

        public MyShowsViewModel(DatabaseService databaseService, ITVShowApiService apiService)
        {
            _databaseService = databaseService;
            _apiService = apiService;
            Title = "My Shows";
            MyShows = new ObservableCollection<UserShow>();
        }

        [RelayCommand]
        private async Task LoadShows()
        {
            try
            {
                IsBusy = true;

                var shows = await _databaseService.GetUserShowsAsync();

                MyShows.Clear();
                foreach (var show in shows.OrderByDescending(s => s.DateAdded))
                {
                    MyShows.Add(show);
                }

                HasShows = MyShows.Any();
                ShowCount = MyShows.Count;

                Debug.WriteLine($"Loaded {ShowCount} shows from database");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading shows: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteShow(UserShow show)
        {
            try
            {
                await _databaseService.DeleteUserShowAsync(show);
                MyShows.Remove(show);
                HasShows = MyShows.Any();
                ShowCount = MyShows.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting show: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadShows();
            IsRefreshing = false;
        }
    }
}