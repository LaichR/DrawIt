using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Types;
using Sketch.Utilities;
using Sketch.Interface;

namespace Sketch.Models
{
    internal class AutoRoutingConnectorStrategy: IConnectorStrategy
    {

        readonly ConnectorModel _model;
        Point _start;
        Point _end;
        readonly List<System.Windows.Media.PathFigure> _myPath = new List<System.Windows.Media.PathFigure>();
        readonly RoutingAssistent _routingAssistent = new RoutingAssistent();
        

        ConnectorMoveHelper _movingState;
        double _startAngle;
        double _endAngle;

        public AutoRoutingConnectorStrategy(ConnectorModel model)
        {
            _model = model;
        }

        public ConnectionType ConnectionType => ConnectionType.AutoRouting;

        public bool AllowWaypoints
        {
            get => true;
        }

        public System.Windows.Point ConnectionStart
        {
            get { return _start; }
        }

        public System.Windows.Point ConnectionEnd
        {
            get { return _end; }
        }

        public double StartAngle
        {
            get { return _startAngle; }
        }

        public double EndAngle
        {
            get { return _endAngle; }
        }

        public IEnumerable<System.Windows.Media.PathFigure> ConnectorPath
        {
            get {
                InitalizeBoundSearchStructures();

                _myPath.Clear();
                if ((_model.From == null) || (_model.To == null))
                {
                    return _myPath;
                }

                var waypointIterator = _model;
                waypointIterator.Reset();
                ConnectorDocking startDocking = ConnectorDocking.Undefined;
                ConnectorDocking endDocking = ConnectorDocking.Undefined;
                double endPointRelativePosition = 0.5;
                double startPointRelativePosition = _model.StartPointRelativePosition;
                
                Point startPoint = new Point();
                Point endPoint = new Point();
                bool morePoints = true;
                bool isFirst = true;

                int middlePosIndex = 0;
                var firstPoint = waypointIterator.Current;
                while (morePoints)
                {
                    morePoints = waypointIterator.MoveNext();
                    if( !morePoints)
                    {
                        endPointRelativePosition = _model.EndPointRelativePosition;
                    }
                    var nextPoint = waypointIterator.Current;
                    bool computeLine = true;
                    startDocking = firstPoint.OutgoingDocking;
                    endDocking = nextPoint.IncomingDocking;
                    startPointRelativePosition = firstPoint.OutgoingRelativePosition;
                    endPointRelativePosition = nextPoint.IncomingRelativePosition;
                    while (computeLine)
                    {
                        startPoint = ComputeStartPoint(firstPoint, nextPoint, ref startPointRelativePosition, ref startDocking);   
                        endPoint = ComputeEndPoint(firstPoint, nextPoint, ref endPointRelativePosition, ref endDocking);
                        computeLine = !CheckDocking(startPoint, endPoint, ref startDocking, ref endDocking);
                    }
                    
                    firstPoint.OutgoingDocking = startDocking;
                    firstPoint.OutgoingRelativePosition = startPointRelativePosition;
                    nextPoint.IncomingDocking = endDocking;
                    nextPoint.IncomingRelativePosition = endPointRelativePosition;

                    LineType lineType = (LineType)((int)startDocking << 8 | (int)endDocking);

                    _myPath.Add(ComputePathFigure(startPoint, endPoint, lineType, firstPoint.MiddlePosition, out double startAngle, out double endAngle));
                    

                    if (isFirst)
                    {
                        _start = startPoint;
                        _startAngle = startAngle;
                    }
                    
                    if(!morePoints)
                    {
                        _end = endPoint;
                        _endAngle = endAngle;
                        
                    }
                    middlePosIndex++;
                    firstPoint = nextPoint;
                    isFirst = false;
                }

                



                return _myPath;
            }
        }



        public IConnectorMoveHelper StartMove( Point p)
        {
            this._movingState = new ConnectorMoveHelper(this._model, this, p);
            return this._movingState;
        }
        public IEnumerable<Point> ComputeLinePoints(Point start, Point end, LineType lineType, double middlePosition,
            out double startAngle, out double endAngle)
        {
            //var lineType = (LineType)((int)startDocking << 8 | (int)endDocking);
            var linePoints = _routingAssistent.GetLinePoints(lineType,
                                start, end, middlePosition);

            var inVector = new Vector
            {
                X = linePoints.ElementAt(1).X - linePoints.ElementAt(0).X,
                Y = linePoints.ElementAt(1).Y - linePoints.ElementAt(0).Y
            };

            startAngle = ConnectorUtilities.ComputeAngle(_model.StartPointDocking, inVector);
            var outVector = new Vector
            {
                X = linePoints.Last().X - linePoints.ElementAt(linePoints.Count() - 2).X,
                Y = linePoints.Last().Y - linePoints.ElementAt(linePoints.Count() - 2).Y
            };
            endAngle = ConnectorUtilities.ComputeAngle(_model.EndPointDocking, outVector);
            return linePoints;
        }

        public PathFigure ComputePathFigure(Point start, Point end, LineType lineType, double middlePosition,
            out double startAngle, out double endAngle)
        {
            var linePoints = ComputeLinePoints(start, end, lineType, middlePosition, out startAngle, out endAngle);
            var pf = ConnectorUtilities.GetPathFigureFromPoints(linePoints);
            pf.IsFilled = false;
            pf.IsClosed = false;

            return pf;
        }

        public void ComputeDockingDuringMove(Rect rect, Point p, ref ConnectorDocking currentDocking, ref Point lastPos)
        {
            ConnectorUtilities.ComputeDockingDuringMove(rect, p, ref currentDocking, ref lastPos);
        }

        Point ComputeEndPoint(IConnectable firstPoint, IConnectable nextPoint, ref double relativePos, ref ConnectorDocking docking)
        {
            var fromCenter = ConnectorUtilities.ComputeCenter(firstPoint.Bounds);


            Point end;
            
            if (docking == ConnectorDocking.Undefined)
            {
                end = nextPoint.GetPreferredConnectorEnd(fromCenter, out relativePos, out docking);
            }
            else
            {
                end = nextPoint.GetConnectorPoint(docking, relativePos);
            }
            return end;
            
        }

        Point ComputeStartPoint(IConnectable firstPoint, IConnectable nextPoint, ref double startPointRelativePos, ref ConnectorDocking docking)
        {
            
            var toCenter = ConnectorUtilities.ComputeCenter(nextPoint.Bounds);
            Point start;
            if (docking == ConnectorDocking.Undefined)
            {

                start = firstPoint.GetPreferredConnectorStart(toCenter, out startPointRelativePos, out docking);
            }
            else
            {
                start = firstPoint.GetConnectorPoint(docking, startPointRelativePos);
            }
            return start;
        }

        void InitalizeBoundSearchStructures()
        {
            this._routingAssistent.InitalizeBoundSearchStructures(
                _model.Connectables?.Where(
                    (x) => x != _model.From && 
                    x != _model.To && !(x.GetCustomAttribute<DoNotConsiderForConnectorRoutingAttribute>() != null)));
        }

        bool ExistsIntersectingBounds(Rect r)
        {
            return _routingAssistent.ExistsIntersectingBounds(r);
        }

        bool CheckDocking(Point start, Point end, ref ConnectorDocking startDocking, ref ConnectorDocking endDocking)
        {
            bool ret;
            var lineType = (LineType)((int)startDocking << 8 | (int)endDocking);
            var mid = new Point((start.X / 2 + end.X / 2), (start.Y / 2 + end.Y / 2));
            var r1 = MakeRect(start, mid);
            var r2 = MakeRect(mid, end);
            switch (lineType)
            {

                case LineType.BottomTop:
                case LineType.TopBottom:
                    r1.X -= 3;
                    r1.Width = 6;
                    r2.Width = 6;
                    r2.X = end.X-3;
                    break;
                
                case LineType.LeftRight:
                case LineType.RightLeft:
                    r1.Y -= 3; r1.Height = 6;
                    r2.Y = end.Y - 3; r2.Height = 6;
                    break;
                default:
                    break;
            }

            ret = !ExistsIntersectingBounds(r1) && !ExistsIntersectingBounds(r2);

            if (!ret)
            {
                switch (lineType)
                {
                    case LineType.BottomTop:
                        startDocking = ConnectorDocking.Left;
                        endDocking = ConnectorDocking.Left;
                        break;        
                    case LineType.TopBottom:
                        startDocking = ConnectorDocking.Right;
                        endDocking = ConnectorDocking.Right;
                        break;
                    case LineType.LeftRight:
                        startDocking = ConnectorDocking.Top;
                        endDocking = ConnectorDocking.Top;
                        break;
                    case LineType.RightLeft:
                        startDocking = ConnectorDocking.Bottom;
                        endDocking = ConnectorDocking.Bottom;
                        break;
                    default:
                        ret = true;
                        break;
                }
            }
            return ret;
        }

        static Rect MakeRect(Point p1, Point p2)
        {
            Point p = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            Size s = new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
            return new Rect(p, s);
        }
    }
}
