using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class NextEpisodeBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isNextToWatch && isNextToWatch)
            {
                return Color.FromArgb("#3B82F6"); // Blue border for next episode
            }
            return Color.FromArgb("#2A2A2A"); // Default border
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
