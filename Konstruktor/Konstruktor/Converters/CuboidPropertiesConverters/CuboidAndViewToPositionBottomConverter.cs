using System;
using System.Globalization;
using System.Windows.Data;

using Konstruktor.Controls;

namespace Konstruktor.Converters
{
    public class CuboidAndViewToPositionBottomConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int max = Settings.MaxSize;
            int y = (int)values[4];
            int z = (int)values[5];
            ViewDirection viewDirection = (ViewDirection)values[6];
            bool fullSize = (bool)values[7];
            int factor = fullSize ? Settings.LargeFactor : Settings.SmallFactor;
            bool fromSelection = (bool)values[8];
            int selectId = (int)values[9];
            bool isDragged = (bool)values[10];

            if (fromSelection && !isDragged)
                return (double) (max - 2 * selectId + 1) * factor;

            switch (viewDirection)
            {
                case ViewDirection.TopView:
                    return (double)(factor * y);
                case ViewDirection.FrontView:
                case ViewDirection.BackView:
                case ViewDirection.LeftView:
                case ViewDirection.RightView:
                    return (double)(factor * z);
                default:
                    return (double)(factor * y);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
