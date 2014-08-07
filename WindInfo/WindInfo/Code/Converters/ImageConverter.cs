using Cimbalino.Phone.Toolkit.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Windows;

namespace WindInfo.Code
{
    public class ImageConverter : MultiValueConverterBase
    {
        private object[] _values;
        public override object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _values = values;
            if (values.Any(w => w == null || string.IsNullOrEmpty(w.ToString())))
                return null;

            byte[] imageBytes;
            var path = string.Concat("/Shared/ShellContent/", string.Format("{0}.jpg", string.Join("_", values)));

            using (var iso = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!iso.FileExists(path))
                    return null;

                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, iso))
                {
                    imageBytes = new byte[fileStream.Length];

                    fileStream.Read(imageBytes, 0, imageBytes.Length);
                }
            }

            using (var memoryStream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmapImage = new BitmapImage();

                bitmapImage.SetSource(memoryStream);
                return bitmapImage;
            }




        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return _values;
        }
    }
}
