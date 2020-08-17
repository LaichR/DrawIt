using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Interface;

namespace Sketch.Models
{
    internal class SelfConnectorStrategy: IConnectorStrategy
    {
        class MovingState: IConnectorMoveHelper
        {
            static readonly int DistanceFromEdge = 10;
            SelfConnectorStrategy _parent;
            Point _start;
            double _distance;
            Types.MoveType _moveType;

            public MovingState(SelfConnectorStrategy parent, Point p)
            {
                _parent = parent;
                int i = 0;
                int nrOfSegments = 0;
                PathFigure pf = _parent._myPath.First();

                var start = pf.StartPoint;
                Point end = start;
                nrOfSegments = pf.Segments.Count;
                foreach (var ps in pf.Segments)
                {
                    var ls = ps as LineSegment;
                    if (ls != null)
                    {
                        end = ls.Point;
                        var l = new LineGeometry(start, end);
                        if (l.StrokeContains(ConnectorUtilities.ConnectorPen, p))
                        {
                            break;
                        }
                        start = end;
                        ++i;
                    }
                }
                _distance = _parent._model.MiddlePointRelativePosition;
                if (i == 0)
                {
                    _moveType = Types.MoveType.MoveStartPoint;
                    _start = _parent._start;
                }
                else if (i == 1 && nrOfSegments == 3)
                {
                    _moveType = Types.MoveType.MoveMiddlePoint;
                    _start = SelfConnectorStrategy.GetMiddlePoint(
                                   SelfConnectorStrategy.GetLineTypeFromDocking(_parent._model.StartPointDocking),
                                   _parent._model.ConnectorStart, _parent._model.ConnectorEnd, _parent._model.MiddlePointRelativePosition);

                }
                else if (i == 2 || (i == 1 && nrOfSegments == 2))
                {
                    _moveType = Types.MoveType.MoveEndPoint;
                    _start = _parent._end;
                }
                else
                {
                    _moveType = Types.MoveType.MoveTypeNone;
                    // we end up here if the 
                    //throw new NotSupportedException("don't know how to move connector");
                }
            }

            public System.Windows.Media.Geometry GetGeometry(
                GeometryGroup gg, Types.LineType lineType, Point start, Point end, double distance)
            {
                _distance = distance;
                lineType = (Types.LineType)(((int)ConnectorDocking.Self << 8) | ((int)lineType & 0xFF));               
               
                _parent.ComputePath(lineType, start, end, _distance);
                gg.Children.Clear();
                var pf = _parent._myPath.First();
                var p = pf.StartPoint;
                foreach( var ls in pf.Segments.OfType<LineSegment>())
                {
                    gg.Children.Add(new LineGeometry(p, ls.Point));
                    p = ls.Point;
                }
                var geometry = gg;
                return geometry;
            }

            public void Commit(ConnectorDocking movePointDocking, 
                ConnectorDocking otherPointDocking, 
                Point newPositionStartPoint, 
                Point newPositionEndPoint, double newDistance)
            {
                //_parent._start = newPositionStartPoint;
                //_parent._end = newPositionEndPoint;
                _distance = Math.Max(0.01, newDistance );
                if( _moveType == Types.MoveType.MoveStartPoint)
                {
                    _parent._model.StartPointDocking = movePointDocking;
                    _parent._model.EndPointDocking = otherPointDocking;
                    _parent._model.StartPointRelativePosition =
                        ConnectorUtilities.ComputeRelativePosition(_parent._model.From.Bounds, newPositionStartPoint, movePointDocking); ;
                }
                else if (_moveType == Types.MoveType.MoveMiddlePoint)
                {
                    _parent._model.MiddlePointRelativePosition = _distance;
                }
                else
                {
                    _parent._model.EndPointDocking = movePointDocking;
                    _parent._model.StartPointDocking = otherPointDocking;
                    _parent._model.EndPointRelativePosition =
                            ConnectorUtilities.ComputeRelativePosition(_parent._model.To.Bounds, newPositionStartPoint, movePointDocking);
                }
                System.Diagnostics.Debug.Assert(_parent._model.StartPointDocking != ConnectorDocking.Undefined);
                System.Diagnostics.Debug.Assert(_parent._model.EndPointDocking != ConnectorDocking.Undefined);

            }

            public void ComputeDockingDuringMove(Rect rect, Point p, ref ConnectorDocking currentDocking, ref Point lastPos)
            {
                double top = rect.Top;
                double bottom = rect.Bottom;
                double left = rect.Left;
                double right = rect.Right;

                var newPos = p;
                lastPos.X = ConnectorUtilities.RestrictRange(left, right, lastPos.X);
                lastPos.Y = ConnectorUtilities.RestrictRange(top, bottom, lastPos.Y);
                newPos.X = ConnectorUtilities.RestrictRange(left, right, newPos.X);
                newPos.Y = ConnectorUtilities.RestrictRange(top, bottom, newPos.Y);
                //newPos.Offset( delta.X, delta.Y);
                // in this case we cannot move in y direction unless we change the  point docking
                if (currentDocking == ConnectorDocking.Bottom || currentDocking == ConnectorDocking.Top)
                {
                    // point is within it's original boundary
                    if (newPos.X > (left + DistanceFromEdge) && newPos.X < (right - DistanceFromEdge))
                    {
                        newPos.Y = lastPos.Y; // we are on the line bottom/top; y has not to be adapted
                    }
                    else if (currentDocking == ConnectorDocking.Bottom)
                    {

                        // we may need to change the point docking
                        if (newPos.X <= left + DistanceFromEdge)
                        {
                            currentDocking = ConnectorDocking.Left;
                            newPos.X = left;
                           
                        }
                        else
                        {
                            currentDocking = ConnectorDocking.Right;
                            newPos.X = right;
                        }
                        newPos.Y = bottom - DistanceFromEdge;
                    }
                    else if (currentDocking == ConnectorDocking.Top)
                    {
                        if (newPos.X <= left + DistanceFromEdge)
                        {
                            currentDocking = ConnectorDocking.Left;
                            newPos.X = left;

                        }
                        else
                        {
                            currentDocking = ConnectorDocking.Right;
                            newPos.X = right;
                        }
                        newPos.Y = top + DistanceFromEdge;
                    }
                }
                else if (currentDocking == ConnectorDocking.Left || currentDocking == ConnectorDocking.Right)
                {
                    // point is within it's original boundary
                    if (newPos.Y < bottom - DistanceFromEdge && newPos.Y > top + DistanceFromEdge)
                    {
                        newPos.X = lastPos.X; // we are on the line bottom/top; y has not to be adapted
                    }
                    else if (currentDocking == ConnectorDocking.Left)
                    {
                        // we may need to change the point docking
                        if (newPos.Y < top + DistanceFromEdge)
                        {
                            currentDocking = ConnectorDocking.Top;
                            newPos.Y = top;
                        }
                        else
                        {
                            currentDocking = ConnectorDocking.Bottom;
                            newPos.Y = bottom;
                        }
                        newPos.X = left + DistanceFromEdge;
                    }
                    else if (currentDocking == ConnectorDocking.Right)
                    {
                        if (newPos.Y < top + DistanceFromEdge)
                        {
                            currentDocking = ConnectorDocking.Top;
                            newPos.Y = top;
                        }
                        else
                        {
                            currentDocking = ConnectorDocking.Bottom;
                            newPos.Y = bottom;
                        }
                        newPos.X = right - DistanceFromEdge;
                    }
                }
                lastPos = newPos;
            }

            public Point StartPoint
            {
                get {
                    return _start;
                }
            }

            public ConnectionType ConnectionType
            {
                get { return ConnectionType.StrightLine ; }
            }

            public Types.LineType LineType
            {
                get
                {
                    return SelfConnectorStrategy.GetLineTypeFromDocking(_parent._model.StartPointDocking);
                }
            }

            public double Distance
            {
                get
                {
                    return _distance;
                }
            }

            public Types.MoveType MoveType
            {
                get { return _moveType; }
            }
        }

        ConnectorModel _model;
        Point _start;
        Point _end;

        

        List<System.Windows.Media.PathFigure> _myPath = new List<System.Windows.Media.PathFigure>();

        public SelfConnectorStrategy(ConnectorModel connectorModel)
        {
            _model = connectorModel;
        }



        public System.Windows.Point ConnectionStart
        {
            get { return _start; }
        }

        public System.Windows.Point ConnectionEnd
        {
            get { 

                return _end; 
            }
        }


        public IEnumerable<System.Windows.Media.PathFigure> ConnectorPath
        {
            get {
                ComputeConnectorLine();
                return _myPath; 
            }
        }

        void ComputeConnectorLine()
        {
            _myPath.Clear();

            if (_model.StartPointDocking == ConnectorDocking.Undefined &&
                _model.EndPointDocking == ConnectorDocking.Undefined)
            {
                _model.StartPointDocking = ConnectorDocking.Top;
                _model.StartPointRelativePosition = 0.20;
                _model.EndPointDocking = ConnectorDocking.Top;
                _model.EndPointRelativePosition = 0.80;
                _model.MiddlePointRelativePosition = 0.30;                
            }

            _start = ConnectorUtilities.ComputePoint(_model.From.Bounds, _model.StartPointDocking, _model.StartPointRelativePosition);
            _end = ConnectorUtilities.ComputePoint(_model.To.Bounds, _model.EndPointDocking, _model.EndPointRelativePosition);
            var lt = GetLineTypeFromDocking(_model.StartPointDocking);
            ComputePath(lt, _start, _end, _model.MiddlePointRelativePosition);
           

        }


        public IConnectorMoveHelper StartMove(Point p)
        {
            return new MovingState(this, p);
        }


        public double StartAngle
        {
            get
            {
                var angle = 90.0;
                switch (_model.StartPointDocking)
                {
                    case ConnectorDocking.Top: angle = 270.0;
                        break;
                    case ConnectorDocking.Left: angle = 180;
                        break;
                    case ConnectorDocking.Bottom: angle = 90;
                        break;
                    case ConnectorDocking.Right: angle = 0;
                        break;
                    default:
                        break;
                }
                return angle;
            }
        }

        public double EndAngle
        {
            get
            {
                var angle = 90.0;
                switch (_model.StartPointDocking)
                {
                    case ConnectorDocking.Top: angle = 90.0;
                        break;
                    case ConnectorDocking.Left: angle = 0;
                        break;
                    case ConnectorDocking.Bottom: angle = 270;
                        break;
                    case ConnectorDocking.Right: angle = 180;
                        break;
                    default:
                        break;
                }
                return angle;
            }
        }

        void ComputePath( Types.LineType lineType , Point start, Point end, double distance)
        {
            _myPath.Clear();

            if( lineType == Types.LineType.SelfTop )
            {
                _myPath = TopTopBottomBottomPath(start, end, 1.0, distance);
            }
            else if (lineType == Types.LineType.SelfBottom)
            {
                _myPath = TopTopBottomBottomPath(start, end, -1.0, distance);
            }
            if (lineType == Types.LineType.SelfLeft)
            {
                _myPath = LeftLeftRightRight(start, end, 1.0, distance);
            }
            else if (lineType == Types.LineType.SelfRight)
            {
                _myPath = LeftLeftRightRight(start, end, -1.0, distance);
            }

        }

        List<PathFigure> TopTopBottomBottomPath(Point start, Point end, double sign, double distance)
        {
            List<PathFigure> path = new List<PathFigure>();
            System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection();
            ls.Add(new System.Windows.Media.LineSegment(new Point(start.X, start.Y - sign * 150 * distance), true));
            ls.Add(new System.Windows.Media.LineSegment(new Point(end.X, start.Y - sign * 150 * distance), true));
            ls.Add(new System.Windows.Media.LineSegment(end, true));
            var pf = new System.Windows.Media.PathFigure();
            pf.StartPoint = start;
            pf.Segments = ls;
            path.Add(pf);
            pf.IsClosed = false;
            pf.IsFilled = false;
            return path;
        }



        List<PathFigure> LeftLeftRightRight(Point start, Point end, double sign, double distance)
        {
            List<PathFigure> path = new List<PathFigure>();
            System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection();
            ls.Add(new System.Windows.Media.LineSegment(new Point(start.X - sign * 150 * distance, start.Y), true));
            ls.Add(new System.Windows.Media.LineSegment(new Point(end.X - sign* 150 * distance, end.Y), true));
            ls.Add(new System.Windows.Media.LineSegment(end, true));
            var pf = new System.Windows.Media.PathFigure();
            pf.StartPoint = start;
            pf.Segments = ls;
            path.Add(pf);
            pf.IsClosed = false;
            pf.IsFilled = false;
            return path;
        }

        static Types.LineType GetLineTypeFromDocking( ConnectorDocking startPointDocking)
        {
            Types.LineType lt = Types.LineType.Undefined;
            switch (startPointDocking)
            {
                case ConnectorDocking.Bottom:
                    lt = Types.LineType.SelfBottom;
                    break;
                case ConnectorDocking.Top:
                    lt = Types.LineType.SelfTop;
                    break;
                case ConnectorDocking.Left:
                    lt = Types.LineType.SelfLeft;
                    break;
                case ConnectorDocking.Right:
                    lt = Types.LineType.SelfRight;
                    break;
                default:
                    break;
            }
            return lt;
        }

        static Point GetMiddlePoint( Types.LineType lineType, Point start, Point end, double distance )
        {
            switch( lineType)
            {
                case Types.LineType.SelfBottom:
                    return new Point((start.X + end.X) / 2.0, start.Y + distance * 150);
                case Types.LineType.SelfTop:
                    return new Point((start.X + end.X) / 2.0, start.Y - distance * 150);
                case Types.LineType.SelfLeft:
                    return new Point(start.X - distance*150, (start.Y + end.Y)/2.0);
                case Types.LineType.SelfRight:
                    return new Point(start.X+ distance * 150, start.Y - distance * 150);
                default:
                    throw new NotSupportedException(string.Format("linetype {0} is not supported", lineType));
            }
        }

    }
}
