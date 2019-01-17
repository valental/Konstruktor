using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Konstruktor.Converters
{
    public class CuboidColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool fromSelection = (bool)values[0];
            bool isSelected = (bool)values[1];

            if (fromSelection)
            {
                if (isSelected)
                    return Brushes.DarkRed;
                else
                    return App.Current.Resources["SelectionCuboidColor"] as SolidColorBrush;
            }
            else
            {
                return Brushes.DarkBlue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
