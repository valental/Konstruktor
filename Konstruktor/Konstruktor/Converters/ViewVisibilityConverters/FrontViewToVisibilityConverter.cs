using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Konstruktor.Controls;

namespace Konstruktor.Converters
{
    public class FrontViewToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ViewDirection)value == ViewDirection.FrontView ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
