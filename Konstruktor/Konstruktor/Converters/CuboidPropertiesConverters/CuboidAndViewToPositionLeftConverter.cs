using System;
using System.Globalization;
using System.Windows.Data;

using Konstruktor.Controls;
using Konstruktor.DataHelpers;

namespace Konstruktor.Converters
{
    public class CuboidAndViewToPositionLeftConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int max = Settings.MaxSize;
            int width = (int)values[0];
            int depth = (int)values[1];
            int x = (int)values[3];
            int y = (int)values[4];
            ViewDirection viewDirection = (ViewDirection)values[6];
            bool fullSize = (bool)values[7];
            int factor = fullSize ? Settings.LargeFactor : Settings.SmallFactor;
            bool fromSelection = (bool)values[8];
            bool isDragged = (bool)values[10];

            if(fromSelection && !isDragged)
                return (double) (max + 1) * factor + 1;

            switch (viewDirection)
            {
                case ViewDirection.TopView:
                case ViewDirection.FrontView:
                    return (double)(factor * x);
                case ViewDirection.BackView:
                    return (double)(factor * (max - x - width));
                case ViewDirection.LeftView:
                    return (double)(factor * (max - y - depth));
                case ViewDirection.RightView:
                    return (double)(factor * y);
                default:
                    return (double)(factor * x);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
