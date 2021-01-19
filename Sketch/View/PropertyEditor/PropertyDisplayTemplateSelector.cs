using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sketch.View.PropertyEditor
{
    public class PropertyDisplayTemplateSelector : DataTemplateSelector
    {

        static readonly Dictionary<Type, string> _typeToTemplateMapping = new Dictionary<Type, string>()
        {
            {typeof(string), "StringValueTemplate" },
            {typeof(short), "IntegerValueTemplate" },
            {typeof(int), "IntegerValueTemplate"  },
            {typeof(long), "IntegerValueTemplate" },
            {typeof(bool), "BoolValueTemplate" },
            {typeof(Rect), "RectValueTemplate" },
            {typeof(System.Windows.Media.Color), "ColorDisplayTemplate" },
            {typeof(double), "DoubleValueTemplate" },
            {typeof(Enum), "StringDisplayTemplate" }
        };

        public static void RegisterDataTemplate(Type dataType, string newTemplateName)
        {
            if (_typeToTemplateMapping.TryGetValue(dataType, out string _1))
            {
                throw new ApplicationException( $"Template for type <{dataType}> already defined" );
            }
            _typeToTemplateMapping.Add(dataType, newTemplateName);
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (item != null && item is PropertyValueModel model)
            {
                if ( model.IsCellTemplateSpecified )
                {
                    return element.FindResource(model.CellTemplateName) as DataTemplate;
                }
                if (_typeToTemplateMapping.TryGetValue(model.PropertyType, out string key))
                {
                    var template =  element.FindResource(key) as DataTemplate;
                    return template;
                }
                if (model.PropertyType == typeof(FontWeight))
                {
                    return element.FindResource("StringDisplayTemplate") as DataTemplate;
                }
                if (model.PropertyType.IsEnum)
                {
                    return element.FindResource("StringDisplayTemplate") as DataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
