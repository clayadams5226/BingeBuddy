using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class BoolToWatchColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isWatched)
            {
                return isWatched ? Color.FromArgb("#10B981") : Color.FromArgb("#6B7280");
            }
            return Color.FromArgb("#6B7280");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
