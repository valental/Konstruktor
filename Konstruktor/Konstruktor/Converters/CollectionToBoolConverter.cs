using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Konstruktor.Converters
{
    public class CollectionToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ICollection collection = value as ICollection;
            return collection.Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
