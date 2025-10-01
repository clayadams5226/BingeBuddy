using System.Globalization;
using Microsoft.Maui.Graphics;

namespace BingeBuddy.Helpers
{
    public class AddedButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAdded)
                return isAdded ? Color.FromArgb("#4B5563") : Color.FromArgb("#3B82F6");
            return Color.FromArgb("#3B82F6");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}