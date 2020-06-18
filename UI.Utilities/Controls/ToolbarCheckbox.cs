using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bluebottle.Base.Controls
{
    public class ToolbarCheckBox : CheckBox
    {
        string _checkedText = "";
        string _uncheckedText = "";
        
        static ToolbarCheckBox()
        {
            IsCheckedProperty.OverrideMetadata(typeof(ToolbarCheckBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsCheckedPropertyChanged)));
        }

        public ToolbarCheckBox()
        {
            this.Margin = new Thickness(5, 5, 0, 0);
            Text = UncheckedText;
        }

        public ToolbarCheckBox(object viewModel, string checkedText, string uncheckedText, Dictionary<string, DependencyProperty> bindings)
        {
            this.Margin = new Thickness(5, 5, 0, 0);
            _checkedText = checkedText;
            _uncheckedText = uncheckedText;
            Text = UncheckedText;
            
            if (bindings != null)
            {
                foreach (var b in bindings)
                {
                    this.SetBinding(b.Value, new Binding(b.Key));
                }
            }
            DataContext = viewModel;
        }

        public string Text
        {
            set { Content = value; }
            get { return (string)Content; }
        }

        public string CheckedText
        {
            get
            {
                return _checkedText;
                //return base.GetValue(CheckedTextProperty) as string;
            }
            set
            {
                //base.SetValue(CheckedTextProperty, value);
            }
        }

        public string UncheckedText
        {
            get
            {
                return _uncheckedText;
                //return base.GetValue(CheckedTextProperty) as string;
            }
            set
            {
                //base.SetValue(CheckedTextProperty, value);
            }
        }

        private static void IsCheckedPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var cb = source as ToolbarCheckBox;
            var isChecked = Convert.ToBoolean(args.NewValue);
            if (cb != null)
            {
                if (isChecked)
                    cb.Text = cb.CheckedText;
                else
                    cb.Text = cb.UncheckedText;
            }
        }
    }
}
