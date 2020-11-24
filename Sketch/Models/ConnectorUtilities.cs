using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Sketch.Controls;
using Sketch.Interface;
using Sketch.Types;

namespace Sketch.Models
{
    internal static class ConnectorUtilities
    {
        internal static readonly Pen ConnectorPen = new Pen(Brushes.White, ComputeConnectorLine.LineWidth);

        public static readonly Vector HorizontalVector = new Vector(100, 0);
        

        public static PathFigure GetPathFigureFromPoints( IEnumerable<Point> linePoints )
        {
            var pf = new PathFigure();
            System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection();
            var start = linePoints.First();
            {
                foreach (var p in linePoints.Skip(1))
                {
                    var lineSegement = new System.Windows.Media.LineSegment(p, true);
                    ls.Add(lineSegement);
                }
            }
            pf.StartPoint = start;
            pf.Segments = ls;
            pf.IsClosed = false;
            pf.IsFilled = false;
            return pf;
        }

        public static IEnumerable<ConnectorModel> GetConnectorsWithSameEndpoints(IEnumerable<ConnectorModel> siblings, 
            IBoundedItemModel from, IBoundedItemModel to, Point start, Point end)
        {
            List<ConnectorModel> sameEndpoints = new List<ConnectorModel>();
            if (siblings != null)
            {
                foreach (var x in siblings)
                {
                    if ((x.From == from && x.To == to) || (x.To == from && x.From == to)) // same connection, eventually reversed
                    {
                        if ((x.ConnectorStart == start && x.ConnectorEnd == end) ||
                            x.ConnectorStart == end && x.ConnectorEnd == start)
                        {
                            sameEndpoints.Add(x);
                        }
                    }
                }
            }
            return sameEndpoints;
        }

        public static ConnectorDocking ComputeDocking(Rect r, Point p)
        {
            if (p.X <= r.Left) return ConnectorDocking.Left;
            if (p.X >= r.Right) return ConnectorDocking.Right;
            if (p.Y <= r.Top ) return ConnectorDocking.Top;
            if (p.Y >= r.Bottom) return ConnectorDocking.Bottom;
            return ConnectorDocking.Undefined;
        }

        public static IConnectorStrategy GetConnectionType( ConnectorModel parent, ConnectionType type, Point s, Point e )
        {
            if( parent.From == parent.To)
            {
                return CreateSelfConnector(parent);
            }
            switch( type)
            {
                case ConnectionType.AutoRouting:
                    return CreateAutoRoutingConnector(parent);
                case ConnectionType.RoutingWithWaypoint:
                    return CreateRoutingWithWaypointsConnector(parent, s, e);
                case ConnectionType.StrightLine:
                    return CreateStrightLineConnector(parent, s, e);
                default:
                    throw new NotSupportedException(string.Format("connection type {0} not supported", type ));
            }
        }

        public static Point ComputeCenter( Rect r )
        {
            var center = new Point()
            {
                X = r.Left + r.Width / 2,
                Y = r.Top + r.Height / 2,
            };
            return center;
        }

        public static Point Intersect( Rect r, Point a, Point b)
        {                                 
            Point inner = a;
            Point outer = b;

            if (r.Contains(b))
            {
                inner = b;
                outer = a;
            }

            var slope = Slope(inner, outer); 
            
            if( inner.Y < r.Bottom && outer.Y > r.Bottom) // might intersect with bottom line of rectangle
            {
                var deltaY = r.Bottom - inner.Y;        // this will be a negative number!
                var deltaX = deltaY / slope;        // if the slope is negaive, delta x will be positive 
                var intersectX = inner.X + deltaX;
                var intersectY = r.Bottom;
                if( intersectX >=r.Left && intersectX <= r.Right) return new Point(intersectX, intersectY); 
            }

            if ( inner.Y > r.Top && outer.Y < r.Top) // might intersect with top line of rectable
            {
                var deltaY = inner.Y - r.Top;
                var deltaX = deltaY / slope;
                var intersectX = inner.X - deltaX;
                var intersectY = r.Top;
                if (intersectX >= r.Left && intersectX <= r.Right) return new Point(intersectX, intersectY); 
            }
            
            if( inner.X < r.Right && outer.X > r.Right) // might intersect with right boundary of rectangle
            {
                var deltaX = r.Right - inner.X;
                var deltaY = slope * deltaX;
                var intersectX = r.Right;
                var intersectY = inner.Y + deltaY;
                // since we already tested the intesections with the bottom and top line, no addtional test is needed
                return new Point(intersectX, intersectY);
            }

            if( inner.X > r.Left && outer.X < r.Left) // might intersect with left boundary of rectangle
            {
                var deltaX = inner.X - r.Left;
                var intersectY = inner.Y - deltaX *slope;               
                var intersectX = r.Left;
                // since we already tested the intesections with the bottom and top line, no addtional test is needed
                return new Point(intersectX, intersectY);
            }
            return r.TopLeft;
        }

        public static double Slope(Point p1, Point p2)
        {
            return (p1.Y - p2.Y) / (p1.X - p2.X); // achtung vorzeichen!
        }

        public static RelativePosition ComputeRelativePositionOfPoints(Point startPoint, Point endPoint)
        {
            var relPos = RelativePosition.Undefined;
            if( startPoint.X > endPoint.X)
            {
                relPos |= RelativePosition.W;
            }
            else if(startPoint.X < endPoint.X)
            {
                relPos |= RelativePosition.E;
            }

            if( startPoint.Y > endPoint.Y)
            {
                relPos |= RelativePosition.N;
            }
            else if( startPoint.Y < endPoint.Y)
            {
                relPos |= RelativePosition.S;
            }
            return relPos;
        }

        public static RelativePosition ComputeRelativePosition( Rect from, Rect to)
        {
            var relPos = RelativePosition.Undefined; // it's a self transtion
            if (from == to || from.IntersectsWith(to)) return relPos;
            const double overlapFraction = 4.0;
            var wPartition = from.Width / overlapFraction;
            if( to.Right >= (from.Right + wPartition))
            {
                relPos |= RelativePosition.E;
            }

            if ( to.Left <= (from.Left - wPartition))
            {
                relPos |= RelativePosition.W;
            }

            var hPartition = from.Height / overlapFraction;
            if ( to.Bottom >= from.Bottom + hPartition)
            {
                relPos |= RelativePosition.S;
            }
            if( to.Top <= from.Top - hPartition)
            {
                relPos |= RelativePosition.N;
            }
            return relPos;

        }

        public static ConnectorDocking ComputeDocking( Rect r, Point p, ref double relativePosition)
        {
            if (p.Y == r.Top || p.Y == r.Bottom)
            {
                relativePosition = (p.X - r.Left)/r.Width;
                if (p.Y == r.Bottom)
                    return ConnectorDocking.Bottom;
                return ConnectorDocking.Top;
            }

            if (p.X == r.Left || p.X == r.Right)
            {
                relativePosition = (p.Y - r.Top) / r.Height;
                if (p.X == r.Right) return ConnectorDocking.Right;
                return ConnectorDocking.Left;
            }
            throw new InvalidOperationException("point is not on the border of rectangle");
        }

        public static ConnectorDocking ComputeDocking( Rect r, double relativePosition, ref Point p)
        {
            
            if (p.Y == r.Top || p.Y == r.Bottom)
            {
                p.X = RoundToGrid(r.Left + relativePosition * r.Width);
                if (p.Y == r.Bottom)
                    return ConnectorDocking.Bottom;
                return ConnectorDocking.Top;
            }

            if (p.X == r.Left || p.X == r.Right)
            {
                p.Y = RoundToGrid(r.Top + relativePosition * r.Height);
                if (p.X == r.Right) return ConnectorDocking.Right;
                return ConnectorDocking.Left;
            }
            throw new InvalidOperationException("point is not on the border of rectangle");
        }

        public static Point ComputePoint( Rect r, ConnectorDocking docking, double relativePosition)
        {
            switch(docking)
            {
                case ConnectorDocking.Top:
                    return new Point(RoundToGrid(r.Left + relativePosition * r.Width), r.Top -1);
                case ConnectorDocking.Right:
                    return new Point(r.Right +1, RoundToGrid(r.Top + relativePosition*r.Height) );
                case ConnectorDocking.Bottom:
                    return new Point(RoundToGrid(r.Left + relativePosition * r.Width), r.Bottom + 1);
                case ConnectorDocking.Left:
                    return new Point(r.Left-1, RoundToGrid(r.Top + relativePosition*r.Height) );
                    
                default:
                    return new Point(RoundToGrid(r.Left + relativePosition * r.Width), r.Top);
                    
            }
        }

        public static double ComputeRelativePosition( Rect r, Point p, ConnectorDocking docking)
        {
            switch( docking )
            {
                case ConnectorDocking.Self:
                    return 0.2;
                case ConnectorDocking.Bottom:
                case ConnectorDocking.Top:
                    return (p.X - r.Left) / r.Width;
                case ConnectorDocking.Left:
                case ConnectorDocking.Right:
                    return (p.Y - r.Top) / r.Height;
                default:
                    throw new NotSupportedException(string.Format("relative position of connector docking {0} not defined", docking));

            }
        }

        public static double ComputeAngle( ConnectorDocking _1, Vector ending)
        {
            return Vector.AngleBetween(HorizontalVector, ending);            
        }


        static IConnectorStrategy CreateSelfConnector( ConnectorModel connectorModel)
        {
            return new SelfConnectorStrategy(connectorModel);
        }

        static IConnectorStrategy CreateAutoRoutingConnector( ConnectorModel connectorModel )
        {
            return new AutoRoutingConnectorStrategy(connectorModel);
        }

        static IConnectorStrategy CreateStrightLineConnector(ConnectorModel connectorModel, Point start, Point end)
        {
            return new StraightLineConnectorStrategy(connectorModel, start, end);
        }

        static IConnectorStrategy CreateRoutingWithWaypointsConnector( ConnectorModel _0,
            Point _1, Point _2)
        {
            return null;//new RoutingConnectorWithWaypointsStrategy(model, startHint, endHint);
            
        }

        public static double RoundToGrid(double number)
        {
            return Math.Round(number /  SketchPad.GridSize) * SketchPad.GridSize;
        }

        public static Point RoundToGrid(Point p)
        {
            p.X = RoundToGrid(p.X);
            p.Y = RoundToGrid(p.Y);
            return p;
        }


        internal static bool HitLineSegment(Point p, PathFigure pathFigure, out int index, out Point start, out Point end)
        {
            LineGeometry lineGeometry = new LineGeometry();
            start = pathFigure.StartPoint;
            end = ((LineSegment)pathFigure.Segments.OfType<LineSegment>().Last()).Point;
            lineGeometry.StartPoint = start;
            index = 0;
            foreach (var lineSegment in pathFigure.Segments.OfType<LineSegment>())
            {
                lineGeometry.EndPoint = lineSegment.Point;
                if (lineGeometry.StrokeContains(ConnectorModel.HittestPen, p))
                {

                    return true;
                }
                index++;
                lineGeometry.StartPoint = lineGeometry.EndPoint;
            }
            index = -1;
            return false;
        }

        #region internal helper
        internal static void ComputeDockingDuringMove(Rect rect, Point p, ref ConnectorDocking currentDocking, ref Point lastPos)
        {
            double top = rect.Top;
            double bottom = rect.Bottom;
            double left = rect.Left;
            double right = rect.Right;

            var newPos = p;
            //lastPos.X = RestrictRange(left, right, lastPos.X);
            //lastPos.Y = RestrictRange(top, bottom, lastPos.X);
            newPos.X = RestrictRange(left, right, newPos.X);
            newPos.Y = RestrictRange(top, bottom, newPos.Y);
            //newPos.Offset( delta.X, delta.Y);
            // in this case we cannot move in y direction unless we change the  point docking
            if (currentDocking == ConnectorDocking.Bottom || currentDocking == ConnectorDocking.Top)
            {
                // point is within it's original boundary
                if (newPos.X > left && newPos.X < right)
                {
                    newPos.Y = lastPos.Y; // we are on the line bottom/top; y has not to be adapted
                }
                else if (currentDocking == ConnectorDocking.Bottom)
                {

                    // we may need to change the point docking
                    if (newPos.X <= left)
                    {
                        currentDocking = BestDockingBottomLeft(rect, ref newPos);
                    }
                    else
                    {
                        currentDocking = BestDockingBottomRight(rect, ref newPos);
                    }
                }
                else if (currentDocking == ConnectorDocking.Top)
                {
                    if (newPos.X <= left + 5)
                    {
                        currentDocking = BestDockingTopLeft(rect, ref newPos);
                    }
                    else
                    {
                        currentDocking = BestDockingTopRight(rect, ref newPos);
                    }
                }
            }
            else if (currentDocking == ConnectorDocking.Left || currentDocking == ConnectorDocking.Right)
            {
                // point is within it's original boundary
                if (newPos.Y < bottom && newPos.Y > top)
                {
                    newPos.X = lastPos.X; // we are on the line bottom/top; y has not to be adapted
                }
                else if (currentDocking == ConnectorDocking.Left)
                {
                    // we may need to change the point docking
                    if (newPos.Y <= top)
                    {
                        currentDocking = BestDockingTopLeft(rect, ref newPos);
                    }
                    else
                    {
                        currentDocking = BestDockingBottomLeft(rect, ref newPos);
                    }
                }
                else if (currentDocking == ConnectorDocking.Right)
                {
                    if (newPos.Y <= top)
                    {
                        currentDocking = BestDockingTopRight(rect, ref newPos);
                    }
                    else
                    {
                        currentDocking = BestDockingBottomRight(rect, ref newPos);
                    }
                }
            }
            lastPos = newPos;
        }

        internal static double RestrictRange(double lowLimit, double highLimit, double value)
        {
            if (value < lowLimit) return lowLimit;
            if (value > highLimit) return highLimit;
            return value;
        }

        internal static Point RestrictRange( Rect rect, Point p)
        {
            var x = ConnectorUtilities.RestrictRange(rect.Left, rect.Right, p.X);
            var y = ConnectorUtilities.RestrictRange(rect.Top, rect.Bottom, p.Y);
            return new Point(x, y);
        }

        internal static Point RestrictRange(Rect rect, Point p, 
            double marginX, double marginY)
        {
            var x = ConnectorUtilities.RestrictRange(rect.Left + marginX, rect.Right-marginX, p.X);
            var y = ConnectorUtilities.RestrictRange(rect.Top-marginY, rect.Bottom+marginY, p.Y);
            return new Point(x, y);
        }

        static ConnectorDocking BestDockingBottomLeft(Rect r, ref Point p)
        {
            Point pBottom = r.BottomLeft; pBottom.Offset(2, 0);
            Point pLeft = r.BottomLeft; pLeft.Offset(0, -2);
            var vBottom = pBottom - p;
            var vLeft = pLeft - p;
            if (vBottom.Length <= vLeft.Length)
            {
                p.Y = pBottom.Y + 1;
                p.X = RestrictRange(r.Left, r.Right, p.X);
                return ConnectorDocking.Bottom;
            }

            p.X = pLeft.X;
            p.Y = RestrictRange(r.Top, r.Bottom, p.Y);
            return ConnectorDocking.Left;
        }

        static ConnectorDocking BestDockingBottomRight(Rect r, ref Point p)
        {
            Point pBottom = r.BottomRight; pBottom.Offset(-2, 0);
            Point pRight = r.BottomRight; pRight.Offset(0, -2);
            var vBottom = pBottom - p;
            var vRight = pRight - p;
            if (vBottom.Length <= vRight.Length)
            {
                p.Y = pBottom.Y + 1;
                p.X = RestrictRange(r.Left, r.Right, p.X);
                return ConnectorDocking.Bottom;
            }

            p.X = pRight.X;
            p.Y = RestrictRange(r.Top, r.Bottom, p.Y);
            return ConnectorDocking.Right;
        }

        static ConnectorDocking BestDockingTopLeft(Rect r, ref Point p)
        {
            Point pTop = r.TopLeft; pTop.Offset(2, 0);
            Point pLeft = r.TopLeft; pLeft.Offset(0, 2);
            var vTop = pTop - p;
            var vLeft = pLeft - p;
            if (vTop.Length <= vLeft.Length)
            {
                p.Y = pTop.Y - 1;
                p.X = RestrictRange(r.Left, r.Right, p.X);
                return ConnectorDocking.Top;
            }

            p.X = pLeft.X;
            p.Y = RestrictRange(r.Top, r.Bottom, p.Y);
            return ConnectorDocking.Left;
        }

        static ConnectorDocking BestDockingTopRight(Rect r, ref Point p)
        {
            Point pTop = r.TopRight; pTop.Offset(-2, 0);
            Point pRight = r.TopRight; pRight.Offset(0, 2);
            var vTop = pTop - p;
            var vRight = pRight - p;
            if (vTop.Length <= vRight.Length)
            {
                p.Y = pTop.Y - 1;
                p.X = RestrictRange(r.Left, r.Right, p.X);
                return ConnectorDocking.Top;
            }

            p.X = pRight.X;
            p.Y = RestrictRange(r.Top, r.Bottom, p.Y);
            return ConnectorDocking.Right;
        }
        #endregion


    }
}
