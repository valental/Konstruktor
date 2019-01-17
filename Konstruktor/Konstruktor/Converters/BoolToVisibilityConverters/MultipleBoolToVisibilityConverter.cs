using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Konstruktor.Converters
{
    public class MultipleBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return Visibility.Collapsed;
            return ((bool)values[0] && (bool)values[1]) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
