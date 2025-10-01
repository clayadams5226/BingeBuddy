using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace BingeBuddy.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private bool isRefreshing;

        public BaseViewModel()
        {

        }
    }
}