using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Sketch.Controls
{
    internal class ConnectorMoveVisualizer: Shape
    {
        Geometry _myGeometry;

        public ConnectorMoveVisualizer(Geometry geometry)
        {
            _myGeometry = geometry;
            this.StrokeThickness = 1;
            this.Stroke = Brushes.Black;
            this.StrokeDashArray = new DoubleCollection{4, 2};
            this.Visibility = System.Windows.Visibility.Visible;
            Canvas.SetZIndex(this, 300);
        }

        protected override Geometry DefiningGeometry
        {
            get { return _myGeometry; }
        }

        public void UpdateGeometry( Geometry geometry)
        {
            _myGeometry = geometry;
            InvalidateVisual();
        }
    }
}
