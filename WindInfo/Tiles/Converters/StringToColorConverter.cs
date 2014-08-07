using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WindInfo.Code;

namespace Tiles
{

    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return Colors.Transparent;
            }


            var color = UInt32.Parse(value.ToString().Replace("#", ""), NumberStyles.HexNumber).ToColor();

            if (targetType == typeof(Color))
                return color;

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            var t = value.ToString();
            return t;
        }


    }
}
