using Cimbalino.Phone.Toolkit.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WindInfo.Code
{
    public class PercentageToColorConverter : MultiValueConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Any(v => v == null))
                return null;

            int v1 = (int)values[0];
            int v2 = (int)values[1];

            var val = 100.0f / v2 * v1;

            if (val > 50)
                return new SolidColorBrush(Colors.Green);
            else if (val > 25)
                return new SolidColorBrush(Colors.Yellow);
            else if (val > 10)
                return new SolidColorBrush(Colors.Orange);

            return new SolidColorBrush(Colors.Red);
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
