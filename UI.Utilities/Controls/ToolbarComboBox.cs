using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Bluebottle.Base.Controls
{
    public class ToolbarComboBox : ComboBox
    {
        static ToolbarComboBox()
        {
            PaddingProperty.OverrideMetadata(typeof(ToolbarComboBox), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), new PropertyChangedCallback(PaddingPropertyChanged)));
            //ItemsSourceProperty.OverrideMetadata(typeof(ToolbarComboBox), new FrameworkPropertyMetadata(new List<string>(), new PropertyChangedCallback(ItemSourcePropertyChanged)));
        }

        public ToolbarComboBox() : this(null, null)
        {
        }

        public ToolbarComboBox(object viewModel, Dictionary<string, DependencyProperty> bindings)
        {
            this.Margin = new Thickness(5, 0, 0, 0);
            this.DataContextChanged += ToolbarComboBox_DataContextChanged;
            this.SourceUpdated += ToolbarComboBox_SourceUpdated;
            if (bindings != null)
            {
                foreach (var b in bindings)
                {
                    this.SetBinding(b.Value, new Binding(b.Key));
                }
            }  
            DataContext = viewModel;
            this.IsEnabled = true;
            MinWidth = 50;
            Width = GetPreferredLength();

            //_dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(ToolbarComboBox));
            //_dpd.AddValueChanged(this, ItemSourcePropertyChanged);
        }

        //private static void ItemSourcePropertyChanged(object sender, EventArgs e)
        //{
        //    var cb = sender as ToolbarComboBox;
        //    cb.SetWidth();
        //}

        //private static void ItemSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        //{
        //    var cb = source as ToolbarComboBox;
        //    cb.SetWidth();
        //}

        private static void PaddingPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var cb = source as ToolbarComboBox;
            cb.SetWidth();
        }

        private void ToolbarComboBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var cb = sender as ToolbarComboBox;
            cb.SetWidth();
        }

        private void ToolbarComboBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            var cb = sender as ToolbarComboBox;
            cb.SetWidth();
        }

        private void SetWidth()
        {
            Width = GetPreferredLength();
        }

        private double MeasureText(string text)
        {
            var meas = new FormattedText(text, System.Threading.Thread.CurrentThread.CurrentCulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Brushes.Black);
            return meas.Width;
        }

        private int GetPreferredLength()
        {
            double maxWidth = MinWidth, temp = 0;
            foreach (var obj in Items)
            {
                temp = MeasureText(obj.ToString());

                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
                if (maxWidth > MaxWidth)
                {
                    maxWidth = MaxWidth;
                    break;
                }
            }

            var ret = (int)(maxWidth < MaxWidth ? maxWidth : MaxWidth);
            ret += System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
            ret += (int)(Padding.Left + Padding.Right);
            return ret;
        }
    }
}
