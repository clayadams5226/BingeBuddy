using System.Globalization;

namespace BingeBuddy.Helpers
{
    /// <summary>
    /// Converts a percentage (0-100) to a progress value (0.0-1.0) for ProgressBar
    /// </summary>
    public class ProgressPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double percentage)
            {
                return percentage / 100.0;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                return progress * 100.0;
            }
            return 0.0;
        }
    }
}
