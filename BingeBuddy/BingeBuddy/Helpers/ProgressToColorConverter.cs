using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class ProgressToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                if (progress >= 100)
                    return Color.FromArgb("#10B981"); // Green - Completed
                else if (progress >= 50)
                    return Color.FromArgb("#3B82F6"); // Blue - In Progress (>50%)
                else if (progress > 0)
                    return Color.FromArgb("#F59E0B"); // Orange - Started (<50%)
                else
                    return Color.FromArgb("#6B7280"); // Gray - Not Started
            }
            return Color.FromArgb("#6B7280");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
