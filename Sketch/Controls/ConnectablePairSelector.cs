using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sketch.Controls
{
    /// <summary>
    /// Represents the line that is drawn to select the second connectable Outline
    /// </summary>
    internal class ConnectablePairSelector: Shape
    {
        Point _start;
       
        PathGeometry _myGeometry;

        public ConnectablePairSelector( Point start, Point tmp )
        {
            _start = start;
            
            ComputePath(tmp);
            this.Stroke = Brushes.Black;
            this.StrokeDashArray.Add(5.0);
            this.StrokeDashArray.Add(2.5);

            this.StrokeThickness = 1;
            this.Visibility = Visibility.Visible;
            ComputePath(tmp);
        }

        public Point Start
        {
            get => _start;
        }

        public void ComputePath( Point p)
        {
            List<System.Windows.Media.PathFigure> path = new List<System.Windows.Media.PathFigure>();
            System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection()
            {
                new System.Windows.Media.LineSegment(p, true)
            };
           
            var pf = new System.Windows.Media.PathFigure()
            {
                StartPoint = _start,
                Segments = ls
            };

            path.Add(pf);
            
            _myGeometry = new PathGeometry(path);
            InvalidateVisual();
        }

        protected override System.Windows.Media.Geometry DefiningGeometry
        {
            get { return _myGeometry; }
        }


    }
}
