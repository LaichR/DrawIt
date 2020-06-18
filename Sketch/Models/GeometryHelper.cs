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

        //public static PathFigure GetPathFigureFromPoint(IEnumerable<Point> linePoints)
        //{
        //    var pf = new PathFigure();
        //    System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection();
        //    var start = linePoints.First();
        //    foreach (var p in linePoints.Skip(1))
        //    {
        //        ls.Add(new System.Windows.Media.BezierSegment(start,linePoints.ElementAt(1), linePoints.ElementAt(2),false));
        //    }
        //    pf.StartPoint = start;
        //    pf.Segments = ls;
        //    return pf;
        //}

    }
}
