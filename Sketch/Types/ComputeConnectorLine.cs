using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sketch.Types
{
    delegate IEnumerable<Point> ComputeLinePointsDelegate(Point start, Point end, double distance);

    internal static class ComputeConnectorLine
    {
        public static readonly double NormalDistance = 50.0;
        public static readonly double LineWidth = 10.0;

        public static readonly Dictionary<LineType, ComputeLinePointsDelegate>
           Table = new Dictionary<LineType, ComputeLinePointsDelegate>
            {
                {LineType.TopBottom, TopBottomLine},
                {LineType.BottomTop, BottomTopLine},
                {LineType.LeftRight, LeftRightLine},
                {LineType.RightLeft, RightLeftLine},
                {LineType.LeftTop, LeftRightTopBottomLine},
                {LineType.TopLeft, TopBottomLeftRightLine},
                {LineType.TopRight, TopBottomLeftRightLine},
                {LineType.RightTop, LeftRightTopBottomLine},
                {LineType.BottomLeft, TopBottomLeftRightLine},
                {LineType.LeftBottom, LeftRightTopBottomLine},
                {LineType.BottomRight, TopBottomLeftRightLine},
                {LineType.RightBottom, LeftRightTopBottomLine},
                {LineType.LeftLeft, LeftLeftLine},
                {LineType.RightRight, RightRightLine},
                {LineType.TopTop, TopTopLine},
                {LineType.BottomBottom, BottomBottomLine},
            };
        #region  line computations
        static IEnumerable<Point> RightLeftLine(Point start, Point end, double distance)
        {           
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X*(1-distance) + end.X*distance, Y = start.Y });
            linePoints.Add(new Point { X = start.X*(1-distance) + end.X*distance, Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> LeftRightLine(Point start, Point end, double distance)
        {

            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X * distance + end.X * (1 - distance), Y = start.Y });
            linePoints.Add(new Point { X = start.X * distance + end.X * (1 - distance), Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> TopBottomLine(Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X, Y = (start.Y * (1 - distance) + end.Y * distance) });
            linePoints.Add(new Point { X = end.X, Y = (start.Y * (1 - distance) + end.Y * distance) });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> BottomTopLine(Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X, Y = (start.Y * distance + end.Y * (1 - distance)) });
            linePoints.Add(new Point { X = end.X, Y = (start.Y * distance + end.Y * (1 - distance)) });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> LeftRightTopBottomLine(Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = end.X, Y = start.Y });
            linePoints.Add(end);
            return linePoints;
        }


        static IEnumerable<Point> TopBottomLeftRightLine(Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X, Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> LeftLeftLine(Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            var minX = Math.Min(start.X, end.X) - distance * NormalDistance;
            linePoints.Add(new Point { X = minX, Y = start.Y });
            linePoints.Add(new Point { X = minX, Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> RightRightLine(Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            var maxX = Math.Max(start.X, end.X) + distance*NormalDistance;
            linePoints.Add(new Point { X = maxX, Y = start.Y });
            linePoints.Add(new Point { X = maxX, Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> TopTopLine(Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            var minY = Math.Min(start.Y, end.Y) - distance * NormalDistance;
            linePoints.Add(new Point { X = start.X, Y = minY });
            linePoints.Add(new Point { X = end.X, Y = minY });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> BottomBottomLine(Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            var maxY = Math.Max(start.Y, end.Y) + distance * NormalDistance;
            linePoints.Add(new Point { X = start.X, Y = maxY });
            linePoints.Add(new Point { X = end.X, Y = maxY });
            linePoints.Add(end);
            return linePoints;
        }

        #endregion
    }
}
