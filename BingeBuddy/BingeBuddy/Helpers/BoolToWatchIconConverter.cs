using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class BoolToWatchIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isWatched)
            {
                return isWatched ? "✓" : "○";
            }
            return "○";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
