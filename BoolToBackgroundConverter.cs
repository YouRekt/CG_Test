using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CG_Test
{
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? new SolidColorBrush(Color.FromArgb(64,0,0,255)) : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush && brush == Brushes.AliceBlue)
            {
                return true;
            }
            return false;
        }
    }
}
