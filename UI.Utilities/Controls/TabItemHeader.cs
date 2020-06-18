using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Prism.Mvvm;

namespace Bluebottle.Base.Controls
{
    public class TabItemHeader: StackPanel 
    {
        public static readonly DependencyProperty CmdCloseProperty =
            DependencyProperty.Register("CmdClose", typeof(ICommand), typeof(TabItemHeader),
            new PropertyMetadata(OnCmdCloseChanged));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TabItemHeader), 
            new PropertyMetadata(OnLabelChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabItemHeader),
            new PropertyMetadata(OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TabItemHeader header = target as TabItemHeader;
            var isSelected = Convert.ToBoolean( e.NewValue);
            if (isSelected)
            {
                header._button.Visibility = Visibility.Visible;
            }
            else
            {
                header._button.Visibility = Visibility.Hidden;
            }

        }

        private static void OnLabelChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TabItemHeader header = target as TabItemHeader;
            header._label.Text =  e.NewValue as string;
        }

        private static void OnCmdCloseChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TabItemHeader header = target as TabItemHeader;
            header._button.Command = e.NewValue as ICommand;
        }

        TextBlock _label;
        Button _button;
        StackPanel _content;

        public TabItemHeader(object viewModel, System.Drawing.Bitmap bmp, Dictionary<string, DependencyProperty> bindings) :
            base()
        {
            //_content = new StackPanel();
            _content = this;
            _content.Orientation = System.Windows.Controls.Orientation.Horizontal;
            //this.MinHeight = 25;
            //this.MinWidth = 50;



            //this.SetVerticalOffset(5.0);
            _label = new TextBlock();
            _label.Text = "hello";
            _label.Margin = new Thickness(5, 1, 10, 0);
            _label.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            _label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            _label.FontFamily = new System.Windows.Media.FontFamily("Arial");
            _label.FontSize = 12;
            //_label.FontFamily.
            //elementRow.Height = new GridLength(_label.Height, GridUnitType.Pixel);


            _button = MakeButton(bmp);
            _button.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            _button.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            _content.Children.Add(_label);
            _content.Children.Add(_button);

            DataContext = viewModel;
            ApplyBindings(bindings);

            UseLayoutRounding = true;
            _button.IsHitTestVisible = true;
            this.Background = System.Windows.Media.Brushes.Transparent;
        }





        public ICommand CmdClose
        {
            get
            {
                return GetValue(TabItemHeader.CmdCloseProperty) as ICommand;
            }
            set
            {
                SetValue(TabItemHeader.CmdCloseProperty, value);
            }
        }

        public string Label
        {
            get
            {
                return GetValue(TabItemHeader.LabelProperty) as string;
            }
            set
            {
                SetValue(TabItemHeader.LabelProperty, value);
            }
        }

        public bool IsSelected
        {
            get { return Convert.ToBoolean(GetValue(TabItemHeader.IsSelectedProperty)); }
            set { SetValue(TabItemHeader.IsSelectedProperty, value); }
        }

        Button MakeButton(System.Drawing.Bitmap bitmap)
        {
            var button = new Button();
            var image = new Image();
            image.Source = Bluebottle.Base.ToBitmapSource.Bitmap2BitmapSource(bitmap);
            button.Content = image;
            button.Visibility = System.Windows.Visibility.Hidden;
            button.Background = System.Windows.Media.Brushes.Transparent;
            button.BorderBrush = System.Windows.Media.Brushes.Transparent;
            button.BorderThickness = new Thickness(0);
            button.Height = bitmap.Height;
            button.Width = bitmap.Width;
            return button;
        }

        void ApplyBindings( Dictionary<string, DependencyProperty> bindings)
        {
            if (bindings != null)
            {
                foreach (var b in bindings)
                {
                    this.SetBinding(b.Value, b.Key );
                }
            }
        }



        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            _button.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            
            //var pos = e.GetPosition(this);
            //if (pos.X < 0 || pos.X > this.ActualWidth ||
            //    pos.Y < 0 || pos.Y > this.ActualHeight)
            {
                if (!IsSelected)
                {
                    _button.Visibility = Visibility.Hidden;
                }
                base.OnMouseLeave(e);
            }

        }

    }
}
