using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Sketch.Helper.UiUtilities
{
    public class BitmapToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
        {
            if( value is Bitmap bitmap)
            {
                var source = ToBitmapSource.Bitmap2BitmapSource(bitmap);
                if ( targetType == typeof(System.Windows.Media.ImageSource ))
                {
                    return source;
                }
                var img = new BitmapImage { Source = source };
                return img;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
