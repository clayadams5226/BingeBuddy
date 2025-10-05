using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class NextEpisodeBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isNextToWatch && isNextToWatch)
            {
                return Color.FromArgb("#1E3A8A"); // Dark blue highlight for next episode
            }
            return Color.FromArgb("#1A1A1A"); // Default dark background
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
