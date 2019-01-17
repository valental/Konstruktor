using System;
using System.Globalization;
using System.Windows.Data;

namespace Konstruktor.Converters
{
    public class BoolToCanvasWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)((bool)value ? Settings.LargeFactor * (Settings.MaxSize + Settings.SelectionWidth) : Settings.SmallFactor * Settings.MaxSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
