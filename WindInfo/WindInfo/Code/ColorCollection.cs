using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WindInfo.Code
{
    public class ColorsCollection : ObservableCollection<KeyValuePair<string, string>>
    {
        public ColorsCollection()
        {
            foreach (var color in typeof(PhoneThemeColors).GetProperties(BindingFlags.Static | BindingFlags.Public)
                       .Where(p => p.PropertyType == typeof(Color))
                       .Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(null).ToString())))
            {
                Add(color);
            }

        }

    }

    public static class PhoneThemeColors
    {

        public static Color FromHexa(string hexaColor)
        {
            var color = UInt32.Parse(hexaColor.ToString().Replace("#", ""), NumberStyles.HexNumber).ToColor();
            return color;

        }
        public static Color Lime
        {
            get
            {
                return FromHexa("#FFA4CC00");
            }
        }

        public static Color Green
        {
            get
            {
                return FromHexa("#FF60A917");
            }
        }

        public static Color Emerald
        {
            get
            {
                return FromHexa("#FF008A00");

            }
        }

        public static Color Accent
        {
            get { return Colors.Transparent; }
        }
        public static Color Teal
        {
            get
            {
                return FromHexa("#FF00ABA9");
            }
        }

        public static Color Cyan
        {
            get
            {
                return FromHexa("#FF1BA1E2");
            }
        }

        public static Color Cobalt
        {
            get
            {
                return FromHexa("#FF0050EF");
            }
        }

        public static Color Indigo
        {
            get
            {
                return FromHexa("#FF6A00FF");
            }
        }

        public static Color Violet
        {
            get
            {
                return FromHexa("#FFAA00FF");
            }
        }


        public static Color Pink
        {
            get
            {
                return FromHexa("#FFFF91B8");
            }
        }

        public static Color Magenta
        {
            get
            {
                return FromHexa("#FFD80073");
            }
        }

        public static Color Crimson
        {
            get
            {
                return FromHexa("#FFA20025");
            }
        }

        public static Color Red
        {
            get
            {
                return FromHexa("#FFE51400");
            }
        }

        public static Color Orange
        {
            get
            {
                return FromHexa("#FFFA6800");
            }
        }

        public static Color Amber
        {
            get
            {
                return FromHexa("#FFF0A30A");
            }
        }

        public static Color Yellow
        {
            get
            {
                return FromHexa("#FFD8C100");
            }
        }

        public static Color Brown
        {
            get
            {
                return FromHexa("#FF825A2C");
            }
        }

        public static Color Olive
        {
            get
            {
                return FromHexa("#FF6D8764");
            }
        }

        public static Color Steel
        {
            get
            {
                return FromHexa("#FF647687");
            }
        }

        public static Color Mauve
        {
            get
            {
                return FromHexa("#FF76608A");
            }
        }

        public static Color Sienna
        {
            get
            {
                return FromHexa("#FF7A3B3F");
            }
        }

        public static Color Black
        {
            get
            {
                return FromHexa("#FF000000");
            }
        }
    }


    public class ColorToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var c = UInt32.Parse(value.ToString().Replace("#", ""), NumberStyles.HexNumber).ToColor();
            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }
    }

}
