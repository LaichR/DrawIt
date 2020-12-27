using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Sketch.PropertyEditor
{
    public class RectConverter : IValueConverter
    {
        public RectConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Rect r)
                return new RectangleWrapper(r);
            throw new NotSupportedException(
               string.Format("value of type {0} cannot be edited as a rectangle", value.GetType().Name));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if( value is RectangleWrapper wrapper)
            {
                return wrapper.Wrapped;
            }
            throw new NotSupportedException(
                string.Format("value of type {0} cannot be converted to a rectangle", value.GetType().Name));
        }
    }
}
