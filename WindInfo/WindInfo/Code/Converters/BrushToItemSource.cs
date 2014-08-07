using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WindInfo.Code
{
    public class BrushToItemSource : IValueConverter
    {

        public static ColorsCollection Cls = new ColorsCollection();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color color;
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                color = Colors.Transparent;
            }
            else
                color = PhoneThemeColors.FromHexa(value.ToString());

            if (!Cls.Any(c => c.Value == color.ToString()))
                return Cls.First();

            return Cls.Where(c => c.Value == color.ToString()).FirstOrDefault();

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return ((KeyValuePair<string, string>)value).Value;
            return value;
        }
    }
}
