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

namespace UI.Utilities.Controls
{
    public class ToolbarButton : Button
    {
        public static readonly DependencyProperty BitmapProperty =
            DependencyProperty.Register("ImageBitmap", typeof(System.Drawing.Bitmap), typeof(ToolbarButton),
            new PropertyMetadata(OnBitmapPropertyChanged));

        readonly System.Windows.Controls.Image _image;

        static ToolbarButton()
        {
            IsEnabledProperty.OverrideMetadata(typeof(ToolbarButton), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsEnabledPropertyChanged)));
        }

        public ToolbarButton()
        {
            this.Margin = new Thickness(5, 0, 0, 0);
            _image = new System.Windows.Controls.Image();
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(_image);
            Content = stackPanel;
        }

        public ToolbarButton(object viewModel, Bitmap bitmap, Dictionary<string, DependencyProperty> bindings)
        {
            this.Margin = new Thickness(5, 0, 0, 0);
            _image = new System.Windows.Controls.Image()
            {
                Source = UI.Utilities.ToBitmapSource.Bitmap2BitmapSource(bitmap)
            };
        

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(_image);
            if (bindings != null)
            {
                foreach (var b in bindings)
                {
                    this.SetBinding(b.Value, new Binding(b.Key));
                }
            }
            DataContext = viewModel;
            Content = stackPanel;
            this.IsEnabled = true;
        }


        public ToolbarButton(object viewModel, Bitmap bitmap, TextBox textbox, Dictionary<string, DependencyProperty> bindings)
        {
            _image = new System.Windows.Controls.Image() 
            { 
                Source = UI.Utilities.ToBitmapSource.Bitmap2BitmapSource(bitmap)
            };
            
            var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal};
            stackPanel.Children.Add(_image);
            stackPanel.Children.Add(textbox);
            if (bindings != null)
            {
                foreach (var b in bindings)
                {
                    this.SetBinding(b.Value, new Binding(b.Key));
                }
            }
            DataContext = viewModel;
            Content = stackPanel;
            this.IsEnabled = true;
        }

        public ToolbarButton(object viewModel, Bitmap bitmap, ComboBox combobox, Dictionary<string, DependencyProperty> bindings)
        {
            _image = new System.Windows.Controls.Image()
            {
                Source = UI.Utilities.ToBitmapSource.Bitmap2BitmapSource(bitmap)
            };
            

            var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            stackPanel.Children.Add(_image);
            stackPanel.Children.Add(combobox);
            if (bindings != null)
            {
                foreach (var b in bindings)
                {
                    this.SetBinding(b.Value, new Binding(b.Key));
                }
            }
            DataContext = viewModel;
            Content = stackPanel;
            this.IsEnabled = true;
        }


        public Bitmap ImageBitmap
        {
            get
            {
                return base.GetValue(BitmapProperty) as Bitmap;
            }
            set
            {
                base.SetValue(BitmapProperty, value);
            }
        }

        private static void OnBitmapPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            ToolbarButton button = source as ToolbarButton;
            var bitmap = button.ImageBitmap;
            if (bitmap != null)
            {
                button._image.Source = ToBitmapSource.Bitmap2BitmapSource(bitmap);
                RefreshBitmap(button, button.IsEnabled);
            }
        }

        private static void IsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (source is ToolbarButton button)
            {
                var isEnable = Convert.ToBoolean(args.NewValue);
                if (button != null && button._image.Source != null)
                {
                    RefreshBitmap(button, isEnable);
                }
            }
        }
        private static void RefreshBitmap(ToolbarButton button, bool isEnable)
        {
            if (!isEnable)
            {
                button._image.Source = new FormatConvertedBitmap((BitmapSource)button._image.Source, PixelFormats.Gray32Float, null, 0);
                button.OpacityMask = new ImageBrush(button._image.Source);
            }
            else
            {
                var src = button._image.Source as FormatConvertedBitmap;
                if (src != null)
                {
                    // Set the Source property to the original value.
                    button._image.Source = src.Source;
                    button.OpacityMask = null;
                }
            }
        }
    }
}
