using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using Sketch.Interface;
using Sketch.Types;

namespace Sketch.Utilities
{
    delegate IEnumerable<Point> ComputeLinePointsDelegateExt(RoutingAssistent assistent, Point start, Point end, double distance);
    public class RoutingAssistent
    {
        const double NormalDistance = 50.0;
        const double MinimalDistance = 20.0;


        // since we need to route, we need to have information about the placement of the other items
        
        LinkedAvlTree<BoundsComparer> _horizontallySortedBounds;
        LinkedAvlTree<BoundsComparer> _verticallySortedBounds;

        static readonly Dictionary<LineType, ComputeLinePointsDelegateExt> _linePointsComputation = new Dictionary<LineType, ComputeLinePointsDelegateExt>()
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


        public RoutingAssistent() {}

        public void InitalizeBoundSearchStructures(IEnumerable<IBoundsProvider> bounds)
        {
            _horizontallySortedBounds = new LinkedAvlTree<BoundsComparer>();
            _verticallySortedBounds = new LinkedAvlTree<BoundsComparer>();
            if (bounds != null)
            {
                foreach (var b in bounds)
                {
                    _horizontallySortedBounds.Add(new BoundsComparer(b,
                        BoundsComparer.CompareType.CompareX));

                    _verticallySortedBounds.Add(new BoundsComparer(b,
                        BoundsComparer.CompareType.CompareY));
                }
            }
        }

        public void InitalizeBoundSearchStructures(IList<Rect> bounds)
        {
            _horizontallySortedBounds = new LinkedAvlTree<BoundsComparer>();
            _verticallySortedBounds = new LinkedAvlTree<BoundsComparer>();
            foreach( var r in bounds )
            {
                _horizontallySortedBounds.Add(new BoundsComparer(new RectangleWrapper(r),
                    BoundsComparer.CompareType.CompareX));

                _verticallySortedBounds.Add(new BoundsComparer(new RectangleWrapper(r),
                    BoundsComparer.CompareType.CompareY));
            }
        }

        public IEnumerable<Point> GetLinePoints(LineType lt, Point start, Point end, double distance)
        {
            if( _linePointsComputation.TryGetValue(lt, out ComputeLinePointsDelegateExt fun))
            {
                return fun(this, start, end, distance);
            }
            throw new NotSupportedException(
                string.Format("A line of type {0} is not supported", lt));
        }

        public double FindVerticalRoutingSlotLeft(double right, double top, double bottom)
        {
            
            Rect r = new Rect(new Point(right - 10, Math.Min(top, bottom)), new Size(10, Math.Abs(bottom - top)));
            var p = _horizontallySortedBounds.UpperBound(r);

            while (p != null && p.Data.Right >= r.Left)
            {
                if (p.Data.Item.Bounds.IntersectsWith(r))
                {
                    r.X = p.Data.Left - 15;
                }
                p = p.Previous;
            } 
            
            return r.Right;
        }

        public double FindVerticalRoutingSlotRight(double left, double top, double bottom)
        {   
            Rect r = new Rect(new Point(left, Math.Min(top, bottom)), new Size(10, Math.Abs(bottom - top)));
            var p = _horizontallySortedBounds.LowerBound(r);
                
            while (p != null && p.Data.Left < r.Right)
            {
                if (p.Data.Item.Bounds.IntersectsWith(r))
                {

                    r.X = p.Data.Right + 5;
                }
                p = p.Next;
            }

            return r.Left;
        }

        public double FindHorizontalRoutingSlotTop(double bottom, double left, double right)
        {
           
            Rect r = new Rect(new Point(Math.Min(left, right), bottom - 10), 
                new Size(Math.Abs(right - left), 10));
            var lower = _verticallySortedBounds.UpperBound(r);
            
            var p = lower;
            while (p != null && p.Data.Bottom >= r.Top)
            {
                if (p.Data.Item.Bounds.IntersectsWith(r))
                {

                    r.Y = p.Data.Top - 15; // the height is 10 so we have to move up 15 
                }
                p = p.Previous;
            }
            
            return r.Bottom;
        }

        public double FindHorizontalRoutingSlotBottom(double top, double left, double right)
        {
            Rect r = new Rect(new Point(Math.Min(left,right), top), new Size(Math.Abs(right - left), 8));
            var lower = _verticallySortedBounds.LowerBound(r);

            var p = lower;
            while (p != null && p.Data.Top < r.Bottom)
            {
                if (p.Data.Item.Bounds.IntersectsWith(r))
                {
                    r.Y = p.Data.Bottom + 5;
                }
                p = p.Next;
            }

            return r.Top;
        }

        public bool ExistsIntersectingBounds(Rect r)
        {

            var bounds = _horizontallySortedBounds.LowerBound(r);
            while (bounds != null && bounds.Data.Left < r.Right)
            {
                if( bounds.Data.Item.Bounds.IntersectsWith(r))
                {
                    return true;
                }
                
                bounds = bounds.Next;
            }

            bounds = _horizontallySortedBounds.UpperBound(r);
            while (bounds != null && bounds.Data.Right >= r.Left)
            {
                if (bounds.Data.Item.Bounds.IntersectsWith(r))
                {
                    return true;
                }
                bounds = bounds.Previous;
            }

            return false;
        }


        #region  line computations
        static IEnumerable<Point> RightLeftLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            
            if (start.X < end.X)
            {
                linePoints.Add(start);
                linePoints.Add(new Point { X = start.X * (1 - distance) + end.X * distance, Y = start.Y });
                linePoints.Add(new Point { X = start.X * (1 - distance) + end.X * distance, Y = end.Y });
                linePoints.Add(end);
            }
            else
            {
                var middleX = (start.X + end.X) / 2;
                var middleY = 0.0;
                if( start.Y > end.Y)
                {
                    middleY = assist.FindHorizontalRoutingSlotBottom((start.Y + end.Y)/2, start.X + MinimalDistance, end.X - MinimalDistance);
                }
                else
                {
                    middleY = assist.FindHorizontalRoutingSlotTop((start.Y + end.Y)/2, start.X + MinimalDistance, end.X - MinimalDistance);
                }
                linePoints.AddRange(RightRightLine(assist, start, new Point(middleX, middleY), 0.5));
                linePoints.AddRange(LeftLeftLine(assist, new Point(middleX, middleY), end, 0.5));
            }
            
            return linePoints;
        }

        static IEnumerable<Point> LeftRightLine(RoutingAssistent assist, Point start, Point end, double distance)
        {

            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X * distance + end.X * (1 - distance), Y = start.Y });
            linePoints.Add(new Point { X = start.X * distance + end.X * (1 - distance), Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> TopBottomLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X, Y = (start.Y * (1 - distance) + end.Y * distance) });
            linePoints.Add(new Point { X = end.X, Y = (start.Y * (1 - distance) + end.Y * distance) });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> BottomTopLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X, Y = (start.Y * distance + end.Y * (1 - distance)) });
            linePoints.Add(new Point { X = end.X, Y = (start.Y * distance + end.Y * (1 - distance)) });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> LeftRightTopBottomLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = end.X, Y = start.Y });
            linePoints.Add(end);
            return linePoints;
        }


        static IEnumerable<Point> TopBottomLeftRightLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            linePoints.Add(new Point { X = start.X, Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> LeftLeftLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            var minX = Math.Min(start.X, end.X) - distance * NormalDistance;
            var minX2 = assist.FindVerticalRoutingSlotLeft(Math.Min(start.X, end.X) - MinimalDistance, start.Y, end.Y);
            minX = Math.Min(minX, minX2);
            linePoints.Add(new Point { X = minX, Y = start.Y });
            linePoints.Add(new Point { X = minX, Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> RightRightLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            var maxX2 = assist.FindVerticalRoutingSlotRight(Math.Max(start.X, end.X) + MinimalDistance,
                start.Y, end.Y);
            var maxX = Math.Max(start.X,end.X) + NormalDistance * distance;
            maxX = Math.Max(maxX2, maxX);
            linePoints.Add(new Point { X = maxX, Y = start.Y });
            linePoints.Add(new Point { X = maxX, Y = end.Y });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> TopTopLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            var minY = Math.Min(start.Y, end.Y) - distance * NormalDistance;
            var minY2 = assist.FindHorizontalRoutingSlotTop(Math.Min(start.Y, end.Y) - MinimalDistance,
                start.X, end.X);
            minY = Math.Min(minY, minY2);
            linePoints.Add(new Point { X = start.X, Y = minY });
            linePoints.Add(new Point { X = end.X, Y = minY });
            linePoints.Add(end);
            return linePoints;
        }

        static IEnumerable<Point> BottomBottomLine(RoutingAssistent assist, Point start, Point end, double distance)
        {
            List<Point> linePoints = new List<Point>();
            linePoints.Add(start);
            var maxY = Math.Max(start.Y, end.Y) + distance * NormalDistance;
            var maxY2 = assist.FindHorizontalRoutingSlotBottom(Math.Max(start.Y, end.Y) + MinimalDistance,
                start.X, end.X);
            maxY = Math.Max(maxY, maxY2);
            linePoints.Add(new Point { X = start.X, Y = maxY });
            linePoints.Add(new Point { X = end.X, Y = maxY });
            linePoints.Add(end);
            return linePoints;
        }
    }
    #endregion
}
