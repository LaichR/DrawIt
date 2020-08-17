using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace Sketch.PropertyEditor
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RectViewer : UserControl
    {
        public static readonly DependencyProperty RectangleProperty =
            DependencyProperty.Register("Rectangle", typeof(RectangleWrapper),
       typeof(RectViewer), new PropertyMetadata(OnRectangleChanged));

        RectangleWrapper _rectangleWrapper = new RectangleWrapper(Rect.Empty);
        bool _settingDataContext = false;


        public RectViewer()
        {

            InitializeComponent();
            
        }

        public RectangleWrapper Rectangle
        {
            get => (RectangleWrapper)this.GetValue(RectangleProperty);
            set => this.SetValue(RectangleProperty, value);
        }

        private void RectViewer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                if (e.OldValue != null)
                {
                    if (e.OldValue is RectangleWrapper wrapper)
                    {
                        wrapper.PropertyChanged -= Wrapper_PropertyChanged;
                    }
                }
                if (e.NewValue != null)
                {
                    if (e.NewValue is RectangleWrapper wrapper)
                    {
                        wrapper.PropertyChanged += Wrapper_PropertyChanged;
                    }
                }
            }
        }

        private void Wrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (DataContext != null)
            {
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(DataContextProperty, DataContext, DataContext));
                
            }
        }
        
        private static void OnRectangleChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            if (source is RectViewer rectViewer)
            {
                if (e.NewValue != e.OldValue && !rectViewer._settingDataContext)
                {
                    rectViewer._settingDataContext = true;
                    rectViewer._rectangleWrapper = (RectangleWrapper)e.NewValue;
                    rectViewer._rectangleWrapper.PropertyChanged += rectViewer.Wrapper_PropertyChanged;
                    rectViewer.DataContext = e.NewValue;
                    //rectViewer.InvalidateProperty(DataContextProperty);
                    rectViewer._settingDataContext = false;
                }
            }
        }
    }
}
