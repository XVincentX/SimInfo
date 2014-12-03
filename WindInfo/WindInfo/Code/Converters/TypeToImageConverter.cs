using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WindInfo.Code
{
    public class TypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            string path = "";
            switch (value.ToString())
            {
                case "W":
                    path = "Assets/Wind.jpg";
                    break;
                case "T":
                    path = "Assets/Tim.jpg";
                    break;
                case "V":
                    path = "Assets/Vodafone.jpg";
                    break;
                case "C":
                    path = "Assets/coop.jpg";
                    break;
                case "H":
                    path = "Assets/tre.jpg";
                    break;
                case "N":
                    path = "Assets/Noverca.jpg";
                    break;
                case "Z":
                    path = "Assets/tiscali.jpg";
                    break;
                case "F":
                    path = "Assets/fastweb.jpg";
                    break;
                default:
                    return null;
            }

            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
