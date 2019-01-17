using System;
using System.Globalization;
using System.Windows.Data;

using Konstruktor.Controls;

namespace Konstruktor.Converters
{
    public class ViewToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ViewDirection)value)
            {
                case ViewDirection.TopView:
                    return "Tlocrt:";
                case ViewDirection.FrontView:
                    return "Nacrt:";
                case ViewDirection.BackView:
                    return "Stražnji nacrt:";
                case ViewDirection.LeftView:
                    return "Lijevi bokocrt:";
                case ViewDirection.RightView:
                    return "Desni bokocrt:";
                default:
                    return "Tlocrt:";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
