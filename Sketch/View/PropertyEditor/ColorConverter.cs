using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Sketch.View.PropertyEditor
{
    public class ColorConverter : IValueConverter
    {
        public ColorConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.Color col)
            {
                if (targetType == typeof(System.Windows.Media.Brush))
                {
                    return new SolidColorBrush(col);
                }
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Colors.White;
            if (value is SolidColorBrush brush)
            {
                return brush.Color;
            }
            throw new NotSupportedException("conversion not supporeted");
        }
    }
}
