using Sketch.Interface;
using Sketch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Sketch.Helper.RuntimeCheck;

namespace Sketch.View
{
    class SketchItemDisplayLabel : Shape
    {
        readonly Typeface _typeface = new Typeface("Arial");
        static readonly Brush _fillBrush = new LinearGradientBrush(Colors.LightGray, new Color()
        { A = 0xFF, R = 0xEF, G = 0xEf, B = 0xEf}, 90);
        
        readonly ISketchItemContainer _container;
        readonly Canvas _canvas;
        readonly GeometryGroup _geometry = new GeometryGroup();
        FormattedText _formattedText;

        public SketchItemDisplayLabel( ISketchItemContainer container, Canvas canvas )
        {
            Contract.Requires<ArgumentNullException>(container != null, "Container must not be null");
            Contract.Requires<ArgumentNullException>(canvas != null, "Canvas must not be null");

            DataContext = container;
            SetBinding(TagProperty, "Label");
            
            _container = container;
            _canvas = canvas;
            Stroke = Brushes.Black;
            StrokeThickness = 0.5;
            Fill = _fillBrush;
            _canvas.Children.Add(this);
            Visibility = Visibility.Visible;
            UpdateGeometry();
        }


        internal void UpdateGeometry()
        {
            string text = Tag.ToString();
            var pixelsPerDpi = VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip;
            _formattedText = new FormattedText(text,
               System.Globalization.CultureInfo.CurrentCulture,
               System.Windows.FlowDirection.LeftToRight, _typeface, 12, Brushes.Black, pixelsPerDpi);

            var textGeometry = _formattedText.BuildGeometry(
                new Point(10, 5));
                
                
            List<Point> borderPoints = new List<Point>()
            {
                new Point(0, textGeometry.Bounds.Bottom + 10),
                new Point(textGeometry.Bounds.Right + 10, textGeometry.Bounds.Bottom + 10),
                new Point(textGeometry.Bounds.Right + 30, (textGeometry.Bounds.Bottom + 10)/4),
                new Point(textGeometry.Bounds.Right + 30, 0),
                new Point(0, 0),
            };
            var boundsGemometryPath = GeometryHelper.GetPathFigureFromPoint(borderPoints);
            boundsGemometryPath.IsClosed = true;
            
            _geometry.Children.Clear();
            _geometry.Children.Add(new PathGeometry(new[] { boundsGemometryPath }));
            _geometry.Children.Add(textGeometry);
        }

        protected override Geometry DefiningGeometry => _geometry;


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawText(_formattedText, new Point(10, 5));

        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseDown(e);
        }
    }
}
