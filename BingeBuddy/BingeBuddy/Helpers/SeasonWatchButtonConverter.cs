using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class SeasonWatchButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int watchedCount && parameter is int totalEpisodes)
            {
                return watchedCount >= totalEpisodes ? "Mark Unwatched" : "Mark Watched";
            }
            // Default based on watched count
            return "Mark Season Watched";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
