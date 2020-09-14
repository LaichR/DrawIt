using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sketch.PropertyEditor
{
    public class DoubleConverter : IValueConverter
    {
        public DoubleConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                return value.ToString();
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0.0;
            if (value is string strValue)
            {
                if (string.IsNullOrEmpty(strValue)) return 0.0;

                return double.Parse(strValue);
            }
            throw new NotSupportedException("conversion not supporeted");
        }
    }
}
