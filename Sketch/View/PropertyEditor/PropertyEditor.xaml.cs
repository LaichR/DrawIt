using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sketch.View.PropertyEditor
{
    /// <summary>
    /// Interaction logic for PropertyEditor.xaml
    /// </summary>
    public partial class PropertyEditor : UserControl
    {
        public static readonly DependencyProperty InspectedObjectProperty =
          DependencyProperty.Register("InspectedObject", typeof(object),
              typeof(PropertyEditor), new PropertyMetadata(OnInspectedObjectChanged));

        readonly PropertyEditorModel _model;

        public PropertyEditor()
        {
            InitializeComponent();
            _model = new PropertyEditorModel();
            DataContext = _model;
        }

        public object InspectedObject
        {
            get => GetValue(InspectedObjectProperty);
            set
            {
                SetValue(InspectedObjectProperty, (object)value);
            }
        }

        private static void OnInspectedObjectChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            if (source is PropertyEditor propertyEditor)
            {
                if (e.NewValue != e.OldValue)
                {
                    propertyEditor._model.Wrap(e.NewValue);
                }
            }
        }
    }
}
