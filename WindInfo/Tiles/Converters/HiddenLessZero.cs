using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Tiles
{
    public class HiddenLessZero : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(DateTime))
            {
                if (((DateTime)System.Convert.ChangeType(value, typeof(DateTime))).Date == DateTime.MinValue.Date)
                    return Visibility.Collapsed;
            }
            else
                if ((float)System.Convert.ChangeType(value, typeof(float)) < 0)
                    return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
