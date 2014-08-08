using Cimbalino.Phone.Toolkit.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WindInfo.Code
{
    public class StringJoinConverter : MultiValueConverterBase
    {

        public override object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Any(w => w == null))
                return null;

            return string.Concat("/Shared/ShellContent/", string.Format("{0}.jpg", string.Join("_", values)));
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Path.GetFileNameWithoutExtension(value.ToString()).Split('_');
        }
    }
}
