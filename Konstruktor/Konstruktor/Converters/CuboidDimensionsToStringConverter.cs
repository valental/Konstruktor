using System;
using System.Globalization;
using System.Windows.Data;

namespace Konstruktor.Converters
{
    public class CuboidDimensionsToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)values[0] + " x " + (int)values[1] + " x " + (int)values[2];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
