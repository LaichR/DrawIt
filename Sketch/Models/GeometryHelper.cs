using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace Sketch.Models
{
    public static class GeometryHelper
    {

        public static Geometry MakeLeftToRightTextGeometry(string text, Point pos, Typeface typeFace, 
            double fontSize, Brush brush )
        {

            var pixelsPerDpi = VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip;
            var formattedLable = new FormattedText(text,
               System.Globalization.CultureInfo.CurrentCulture,
               System.Windows.FlowDirection.LeftToRight, typeFace, fontSize, brush, pixelsPerDpi);
            
            var geometry =  formattedLable.BuildGeometry(pos);
            
            return geometry;
        }

        public static Geometry GetGeometryFromPath( params PathFigure[] paths)
        {
            return new PathGeometry(paths);
        }

        public static PathFigure GetPathFigureFromPoint(IEnumerable<Point> linePoints)
        {
            var pf = new PathFigure();
            System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection();
            var start = linePoints.First();
            foreach (var p in linePoints.Skip(1))
            {
                ls.Add(new System.Windows.Media.LineSegment(p, true));
            }
            pf.StartPoint = start;
            pf.Segments = ls;
            return pf;
        }

        public static Geometry GetGeometryFromPoints(IEnumerable<Point> points)
        {
            return GetGeometryFromPath(GetPathFigureFromPoint(points));
        }
    }
}
