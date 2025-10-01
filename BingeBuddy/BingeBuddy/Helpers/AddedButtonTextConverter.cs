using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class AddedButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAdded)
                return isAdded ? "✓" : "+";
            return "+";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}