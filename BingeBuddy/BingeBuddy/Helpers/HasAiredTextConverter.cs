using System.Globalization;

namespace BingeBuddy.Helpers
{
    public class HasAiredTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool hasAired)
            {
                return hasAired ? "" : "UPCOMING";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
