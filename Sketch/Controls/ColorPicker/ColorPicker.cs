//
// ColorPicker.cs 
// An HSB (hue, saturation, brightness) based
// color picker.
//
// 
using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Markup;
using System.Text;


namespace Sketch.Controls.ColorPicker
{


    #region ColorPicker

    public class ColorPicker : Control
    {
        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public ColorPicker()
        {
            templateApplied = false;
            _color = SelectedColor;
            shouldFindPoint = true;
            //SetValue(AProperty, _color.A);
            //SetValue(RProperty, _color.R);
            //SetValue(GProperty, _color.G);
            //SetValue(BProperty, _color.B);
            //SetValue(SelectedColorProperty, _color);
        }


        #region Public Methods
        

        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();
            _colorDetail = GetTemplateChild(ColorDetailName) as FrameworkElement;
            _colorMarker = GetTemplateChild(ColorMarkerName) as Path;
            _colorSlider = GetTemplateChild(ColorSliderName) as SpectrumSlider;
            _colorSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(BaseColorChanged);
            
            
            _colorMarker.RenderTransform = markerTransform;
            _colorMarker.RenderTransformOrigin = new Point(0.5, 0.5);
            _colorDetail.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            _colorDetail.PreviewMouseMove += new MouseEventHandler(OnMouseMove);
            _colorDetail.SizeChanged += new SizeChangedEventHandler(ColorDetailSizeChanged);

            templateApplied = true;
            shouldFindPoint = true;
            isAlphaChange = false;
            _color = SelectedColor;
            SetColor(_color);
            
        }



        #endregion


        #region Public Properties

        // Gets or sets the selected color.
        public System.Windows.Media.Color SelectedColor
        {
            get
            {

                return (System.Windows.Media.Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
                
            }
        }


        #region RGB Properties
        // Gets or sets the ARGB alpha value of the selected color.
        public byte A
        {
            get
            {
                return (byte)GetValue(AProperty);
            }
            set
            {
                SetValue(AProperty, value);
            }
        }

        // Gets or sets the ARGB red value of the selected color.
        public byte R
        {
            get
            {
                return (byte)GetValue(RProperty);
            }
            set
            {
                SetValue(RProperty, value);
            }
        }

        // Gets or sets the ARGB green value of the selected color.
        public byte G
        {
            get
            {
                return (byte)GetValue(GProperty);
            }
            set
            {
                SetValue(GProperty, value);
            }
        }

        // Gets or sets the ARGB blue value of the selected color.
        public byte B
        {
            get
            {
                return (byte)GetValue(BProperty);
            }
            set
            {
                SetValue(BProperty, value);
            }
        }
        #endregion RGB Properties

        #region ScRGB Properties

        // Gets or sets the ScRGB alpha value of the selected color.
        public double ScA
        {
            get
            {
                return (double)GetValue(ScAProperty);
            }
            set
            {
                SetValue(ScAProperty, value);
            }
        }

        // Gets or sets the ScRGB red value of the selected color.
        public double ScR
        {
            get
            {
                return (double)GetValue(ScRProperty);
            }
            set
            {
                SetValue(RProperty, value);
            }
        }

        // Gets or sets the ScRGB green value of the selected color.
        public double ScG
        {
            get
            {
                return (double)GetValue(ScGProperty);
            }
            set
            {
                SetValue(GProperty, value);
            }
        }

        // Gets or sets the ScRGB blue value of the selected color.
        public double ScB
        {
            get
            {
                return (double)GetValue(BProperty);
            }
            set
            {
                SetValue(BProperty, value);
            }
        }
        #endregion ScRGB Properties

        // Gets or sets the the selected color in hexadecimal notation.
        public string HexadecimalString
        {
            get
            {
                return (string)GetValue(HexadecimalStringProperty);
            }
            set
            {
                SetValue(HexadecimalStringProperty, value);
            }
        }

        #endregion


        #region Public Events

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add
            {
                AddHandler(SelectedColorChangedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedColorChangedEvent, value);
            }
        }

        #endregion


        #region Dependency Property Fields
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register
            ("SelectedColor", typeof(System.Windows.Media.Color), typeof(ColorPicker),
            new PropertyMetadata(System.Windows.Media.Colors.White,
                new PropertyChangedCallback(SelectedColor_Changed)
            ));

        public static readonly DependencyProperty ScAProperty =

               DependencyProperty.Register
               ("ScA", typeof(float), typeof(ColorPicker),
               new PropertyMetadata((float)1,
               new PropertyChangedCallback(ScAChanged)
             ));

        public static readonly DependencyProperty ScRProperty =
              DependencyProperty.Register
              ("ScR", typeof(float), typeof(ColorPicker),
              new PropertyMetadata((float)1,
              new PropertyChangedCallback(ScRChanged)
             ));

        public static readonly DependencyProperty ScGProperty =
              DependencyProperty.Register
              ("ScG", typeof(float), typeof(ColorPicker),
              new PropertyMetadata((float)1,
              new PropertyChangedCallback(ScGChanged)
             ));

        public static readonly DependencyProperty ScBProperty =
              DependencyProperty.Register
              ("ScB", typeof(float), typeof(ColorPicker),
              new PropertyMetadata((float)1,
              new PropertyChangedCallback(ScBChanged)
             ));

        public static readonly DependencyProperty AProperty =
              DependencyProperty.Register
              ("A", typeof(byte), typeof(ColorPicker),
              new PropertyMetadata((byte)255,
              new PropertyChangedCallback(AChanged)
             ));

        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register
            ("R", typeof(byte), typeof(ColorPicker),
            new PropertyMetadata((byte)255,
            new PropertyChangedCallback(RChanged)
            ));

        public static readonly DependencyProperty GProperty =
            DependencyProperty.Register
            ("G", typeof(byte), typeof(ColorPicker),
            new PropertyMetadata((byte)255,
            new PropertyChangedCallback(GChanged)
            ));

        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register
            ("B", typeof(byte), typeof(ColorPicker),
            new PropertyMetadata((byte)255,
            new PropertyChangedCallback(BChanged)
            ));

        public static readonly DependencyProperty HexadecimalStringProperty =
            DependencyProperty.Register
            ("HexadecimalString", typeof(string), typeof(ColorPicker),
            new PropertyMetadata("#FFFFFFFF",
            new PropertyChangedCallback(HexadecimalStringChanged)
         ));

        #endregion


        #region RoutedEvent Fields

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedColorChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Color>),
            typeof(ColorPicker)
        );
        #endregion


        #region Property Changed Callbacks

        private static void AChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnAChanged((byte)e.NewValue);
        }

        protected virtual void OnAChanged(byte newValue)
        {

            _color.A = newValue;
            SetValue(ScAProperty, _color.ScA);
            SetValue(SelectedColorProperty, _color);

        }

        private static void RChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnRChanged((byte)e.NewValue);
        }

        protected virtual void OnRChanged(byte newValue)
        {
            _color.R = newValue;
            SetValue(ScRProperty, _color.ScR);
            SetValue(SelectedColorProperty, _color);
        }


        private static void GChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnGChanged((byte)e.NewValue);
        }

        protected virtual void OnGChanged(byte newValue)
        {

            _color.G = newValue;
            SetValue(ScGProperty, _color.ScG);
            SetValue(SelectedColorProperty, _color);
        }


        private static void BChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnBChanged((byte)e.NewValue);
        }

        protected virtual void OnBChanged(byte newValue)
        {
            _color.B = newValue;
            SetValue(ScBProperty, _color.ScB);
            SetValue(SelectedColorProperty, _color);
        }


        private static void ScAChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnScAChanged((float)e.NewValue);
        }

        protected virtual void OnScAChanged(float newValue)
        {
            isAlphaChange = true;
            if (shouldFindPoint)
            {
                _color.ScA = newValue;
                SetValue(AProperty, _color.A);
                SetValue(SelectedColorProperty, _color);
                SetValue(HexadecimalStringProperty, _color.ToString());
            }
            isAlphaChange = false;
        }


        private static void ScRChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnScRChanged((float)e.NewValue);

        }

        protected virtual void OnScRChanged(float newValue)
        {
            if (shouldFindPoint)
            {
                _color.ScR = newValue;
                SetValue(RProperty, _color.R);
                SetValue(SelectedColorProperty, _color);
                SetValue(HexadecimalStringProperty, _color.ToString());
            }
        }


        private static void ScGChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnScGChanged((float)e.NewValue);
        }

        protected virtual void OnScGChanged(float newValue)
        {

            if (shouldFindPoint)
            {
                _color.ScG = newValue;
                SetValue(GProperty, _color.G);
                SetValue(SelectedColorProperty, _color);
                SetValue(HexadecimalStringProperty, _color.ToString());
            }
        }


        private static void ScBChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnScBChanged((float)e.NewValue);
        }

        protected virtual void OnScBChanged(float newValue)
        {
            if (shouldFindPoint)
            {
                _color.ScB = newValue;
                SetValue(BProperty, _color.B);
                SetValue(SelectedColorProperty, _color);
                SetValue(HexadecimalStringProperty, _color.ToString());
            }
        }

        private static void HexadecimalStringChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnHexadecimalStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnHexadecimalStringChanged(string oldValue, string newValue)
        {

            try
            {

                if (shouldFindPoint)
                {

                    _color = (Color)ColorConverter.ConvertFromString(newValue);

                }

                SetValue(AProperty, _color.A);
                SetValue(RProperty, _color.R);
                SetValue(GProperty, _color.G);
                SetValue(BProperty, _color.B);


                if (shouldFindPoint && !isAlphaChange && templateApplied)
                {
                    UpdateMarkerPosition(_color);
                }
            }
            catch (FormatException )
            {

                SetValue(HexadecimalStringProperty, oldValue);
            }

        }

        private static void SelectedColor_Changed(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ColorPicker cPicker = (ColorPicker)d;
            cPicker.OnSelectedColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        protected virtual void OnSelectedColorChanged(Color oldColor, Color newColor)
        {
            if (oldColor != newColor)
            {
                SetColor(newColor);
                RoutedPropertyChangedEventArgs<Color> newEventArgs =
                    new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor);
                    newEventArgs.RoutedEvent = ColorPicker.SelectedColorChangedEvent;
                RaiseEvent(newEventArgs);
            }
        }

        #endregion


        #region Template Part Event Handlers


        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            
            templateApplied = false;
            if (oldTemplate != null)
            {
                _colorSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(BaseColorChanged);
                _colorDetail.MouseLeftButtonDown -= new MouseButtonEventHandler(OnMouseLeftButtonDown);
                _colorDetail.PreviewMouseMove -= new MouseEventHandler(OnMouseMove);
                _colorDetail.SizeChanged -= new SizeChangedEventHandler(ColorDetailSizeChanged);
                _colorDetail = null;
                _colorMarker = null;
                _colorSlider = null;
            }
            base.OnTemplateChanged(oldTemplate, newTemplate);
        }


        private void BaseColorChanged(
            object sender,
            RoutedPropertyChangedEventArgs<Double> e)
        {

            if (_colorPosition != null)
            {

                determineColor((Point)_colorPosition);
            }

        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Point p = e.GetPosition(_colorDetail);
            UpdateMarkerPosition(p);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {


            if (e.LeftButton == MouseButtonState.Pressed)
            {

                Point p = e.GetPosition(_colorDetail);
                UpdateMarkerPosition(p);
                Mouse.Synchronize();

            }
        }

        private void ColorDetailSizeChanged(object sender, SizeChangedEventArgs args)
        {

            if (args.PreviousSize != Size.Empty &&
                args.PreviousSize.Width != 0 && args.PreviousSize.Height != 0)
            {
                double widthDifference = args.NewSize.Width / args.PreviousSize.Width;
                double heightDifference = args.NewSize.Height / args.PreviousSize.Height;
                markerTransform.X *= widthDifference;
                markerTransform.Y *= heightDifference;
            }
            else if (_colorPosition != null)
            {
                markerTransform.X = ((Point)_colorPosition).X * args.NewSize.Width;
                markerTransform.Y = ((Point)_colorPosition).Y * args.NewSize.Height;
            }
        }

        #endregion


        #region Color Resolution Helpers

        private void SetColor(Color theColor)
        {
            _color = theColor;
            
            if (templateApplied) {
                SetValue(AProperty, _color.A);
                SetValue(RProperty, _color.R);
                SetValue(GProperty, _color.G);
                SetValue(BProperty, _color.B);
                UpdateMarkerPosition(theColor);
            }
            
        }

        private void UpdateMarkerPosition(Point p)
        {
            markerTransform.X = p.X;
            markerTransform.Y = p.Y;
            p.X /= _colorDetail.ActualWidth;
            p.Y /= _colorDetail.ActualHeight;
            _colorPosition = p;
            determineColor(p);
        }

        private void UpdateMarkerPosition(Color theColor)
        {
            _colorPosition = null;
            
            
            HsvColor hsv = ColorUtilities.ConvertRgbToHsv(theColor.R, theColor.G, theColor.B);

            _colorSlider.Value = hsv.H;
    
            Point p = new Point(hsv.S, 1 - hsv.V);
            
            _colorPosition = p;
            p.X *= _colorDetail.ActualWidth;
            p.Y *= _colorDetail.ActualHeight;
            markerTransform.X = p.X;
            markerTransform.Y = p.Y;

        }

        private void determineColor(Point p)
        {

            HsvColor hsv = new HsvColor(360 - _colorSlider.Value, 1, 1)
            {
                S = p.X,
                V = 1- p.Y,
            };
           
            _color = ColorUtilities.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);
            shouldFindPoint = false;
            _color.ScA = (float)GetValue(ScAProperty);
            SetValue(HexadecimalStringProperty, _color.ToString());
            shouldFindPoint = true;

        }

        #endregion


        #region Private Fields
        private SpectrumSlider _colorSlider;
        private static readonly string ColorSliderName = "PART_ColorSlider";
        private FrameworkElement _colorDetail;
        private static readonly string ColorDetailName = "PART_ColorDetail";
        private TranslateTransform markerTransform = new TranslateTransform();
        private Path _colorMarker;
        private static readonly string ColorMarkerName = "PART_ColorMarker";
        private Point? _colorPosition;
        private Color _color;
        private bool shouldFindPoint;
        private bool templateApplied;
        private bool isAlphaChange;
        #endregion

    }

    #endregion ColorPicker


}