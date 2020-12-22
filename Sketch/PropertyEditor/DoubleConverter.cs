using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ComponentModel=System.ComponentModel;
using System.Windows.Data;

namespace Sketch.PropertyEditor
{
    public class DoubleConverter : IValueConverter
    {
        readonly ComponentModel.DoubleConverter converter = new ComponentModel.DoubleConverter();
        public DoubleConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return converter.ConvertTo(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return converter.ConvertFrom(value);
        }
    }
}
