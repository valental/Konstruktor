using System;
using System.Globalization;
using System.Windows.Data;

using Konstruktor.Controls;
using Konstruktor.DataHelpers;

namespace Konstruktor.Converters
{
    public class CuboidAndViewToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Cuboid cuboid = (Cuboid)values[0];
            ViewDirection viewDirection = (ViewDirection)values[1];
            bool fullSize = (bool)values[2];
            int factor = fullSize ? Settings.LargeFactor : Settings.SmallFactor;

            if (cuboid.FromSelection && !cuboid.IsDragged)
                return fullSize ? Settings.LargeFactor : 0.0;
            
            switch (viewDirection)
            {
                case ViewDirection.TopView:
                case ViewDirection.FrontView:
                case ViewDirection.BackView:
                    return (double)(factor * cuboid.Width);
                case ViewDirection.LeftView:
                case ViewDirection.RightView:
                    return (double)(factor * cuboid.Depth);
                default:
                    return (double)(factor * cuboid.Width);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
