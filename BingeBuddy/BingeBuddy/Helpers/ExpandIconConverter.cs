using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class ExpandIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                return "▼"; // Always down arrow, use rotation for animation
            }
            return "▼";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
