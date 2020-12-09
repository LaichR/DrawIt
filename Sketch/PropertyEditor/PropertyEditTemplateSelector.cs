using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sketch.PropertyEditor
{
    public class PropertyEditTemplateSelector: DataTemplateSelector
    {

        static readonly Dictionary<Type, string> _typeToTemplateMapping = new Dictionary<Type, string>()
        {
            {typeof(string), "StringValueTemplate" },
            {typeof(short), "IntegerValueTemplate" },
            {typeof(int), "IntegerValueTemplate"  },
            {typeof(long), "IntegerValueTemplate" },
            {typeof(bool), "BoolValueTemplate" },
            {typeof(Rect), "RectValueTemplate" },
            {typeof(double), "DoubleValueTemplate" },
            {typeof(Enum), "SimpleCollectionTemplate" },
            {typeof(System.Windows.Media.Color), "ColorPickerTemplate" }
        };

        public static void RegisterDataTemplate(Type dataType, string newTemplateName)
        {
            if (_typeToTemplateMapping.TryGetValue(dataType, out string _1))
            {
                throw new ApplicationException(
                    string.Format("Template for type <{0}> already defined", dataType));
            }
            _typeToTemplateMapping.Add(dataType, newTemplateName);
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (item != null && item is PropertyValueModel model)
            {
                if (_typeToTemplateMapping.TryGetValue(model.PropertyType, out string key))
                {
                    return element.FindResource(key) as DataTemplate;
                }
                if (model.PropertyType == typeof(FontWeight))
                {
                    return element.FindResource("SimpleCollectionTemplate") as DataTemplate;
                }
                if (model.PropertyType.IsEnum)
                {
                    return element.FindResource("SimpleCollectionTemplate") as DataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
