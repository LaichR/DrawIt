using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Types;
using Sketch.Interface;

namespace Sketch.Models
{
    internal class AutoRoutingConnectorStrategy: IConnectorStrategy
    {


        class MovingState: IConnectorMoveHelper
        {
            MoveType _moveType;
            ConnectorModel _model;
            AutoRoutingConnectorStrategy _parent;
            Pen _connectorPen = new Pen(Brushes.White, ComputeConnectorLine.LineWidth);
            Point _startPoint;
            double _distance;

            public MovingState( ConnectorModel connectorModel, AutoRoutingConnectorStrategy parent, Point p)
            {
                _parent = parent;
                _model = connectorModel;
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
                        if (l.StrokeContains(_connectorPen, p))
                        {
                            break;
                        }
                        start = end;
                        ++i;
                    }
                }
                _distance = _parent._model.MiddlePointRelativePosition;
                if ( i == 0)
                {
                        _moveType = Types.MoveType.MoveStartPoint;
                        _startPoint = _parent._start;
                }
                else if( i == 1 && nrOfSegments == 3)
                {
                        _moveType = Types.MoveType.MoveMiddlePoint;
                        _startPoint = new Point( (start.X + end.X)/2.0, (start.Y + end.Y)/2 );
                        
                }
                else if( i == 2 || (i == 1 && nrOfSegments == 2) )
                {
                    _moveType = Types.MoveType.MoveEndPoint;
                    _startPoint = _parent._end;
                }
                else
                {
                    _moveType = Types.MoveType.MoveTypeNone;
                    //throw new NotSupportedException("don't know how to move connector");
                }                       

            }

            public MoveType MoveType
            {
                get { return _moveType; }
                set { _moveType = value; }
            }

            public Geometry GetGeometry(LineType lineType, Point start, Point end, double distance)
            {
                ComputeLinePointsDelegate computeLinePoints = null;
                if( ComputeConnectorLine.Table.TryGetValue(lineType, out computeLinePoints) )
                {
                    var linePoints = computeLinePoints(start, end, distance);
                    GeometryGroup gg = new GeometryGroup();
                    Point p1 = linePoints.First();
                    foreach (var p2 in linePoints.Skip(1))
                    {
                        var lg = new LineGeometry(p1, p2);
                        gg.Children.Add(lg);
                        p1 = p2;
                    }
                    return gg;
                    //var pf = ConnectorUtilities.GetPathFigureFromPoints(linePoints);
                    //PathGeometry pg = new PathGeometry(new PathFigure[] { pf });
                    //return pg;
                }
                throw new NotSupportedException(
                    string.Format("specified line type {0} cannot be drawn", lineType));
            }

            public void Commit(ConnectorDocking movePointDocking, ConnectorDocking otherPointDocking, Point newMovePointPosition, Point newOtherPointPosition, double newDistance)
            {


                if( _moveType == Types.MoveType.MoveStartPoint)
                {
                    _model.StartPointDocking = movePointDocking;
                    _model.EndPointDocking = otherPointDocking;
                    _model.StartPointRelativePosition = ConnectorUtilities.ComputeRelativePosition(_model.From.Bounds, newMovePointPosition, _model.StartPointDocking);
                    _model.EndPointRelativePosition = ConnectorUtilities.ComputeRelativePosition(_model.To.Bounds, newOtherPointPosition, _model.EndPointDocking);
                }
                else if( MoveType == Types.MoveType.MoveMiddlePoint)
                {
                    _model.MiddlePointRelativePosition = newDistance;
                }
                else if( MoveType == Types.MoveType.MoveEndPoint)
                {
                    _model.StartPointDocking = otherPointDocking;
                    _model.EndPointDocking = movePointDocking;
                    _model.StartPointRelativePosition = ConnectorUtilities.ComputeRelativePosition(_model.From.Bounds, newOtherPointPosition, _model.StartPointDocking);
                    _model.EndPointRelativePosition = ConnectorUtilities.ComputeRelativePosition(_model.To.Bounds, newMovePointPosition, _model.EndPointDocking);
                }
            }

            public Point StartPoint
            {
                get { return _startPoint; }
            }

            public ConnectionType ConnectionType
            {
                get { return ConnectionType.AutoRouting; }
            }

            public LineType LineType
            {
                get {
                    LineType lt = (LineType)((int)_parent._model.StartPointDocking << 8 | (int)_parent._model.EndPointDocking);
                    return lt;
                }
            }

            public double Distance
            {
                get { return _distance; }
            }


            public void ComputeDockingDuringMove(Rect rect, Point p, ref ConnectorDocking currentDocking, ref Point lastPos)
            {
                ConnectorUtilities.ComputeDockingDuringMove(rect, p, ref currentDocking, ref lastPos);
            }
        }


        ConnectorModel _model;
        Point _start;
        Point _end;
        List<System.Windows.Media.PathFigure> _myPath = new List<System.Windows.Media.PathFigure>();
        MovingState _movingState;
        double _startAngle;
        double _endAngle;

        public AutoRoutingConnectorStrategy(ConnectorModel model)
        {
            _model = model;
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

                _myPath.Clear();
                if( (_model.From == null)|| (_model.To == null))
                {
                    return _myPath;
                }
                var fromCenter = ConnectorUtilities.ComputeCenter(_model.From.Bounds);
                var toCenter = ConnectorUtilities.ComputeCenter(_model.To.Bounds);

                if (_model.StartPointDocking == ConnectorDocking.Undefined)
                {
                    
                    _start = ConnectorUtilities.Intersect(_model.From.Bounds, fromCenter, toCenter);
                    _model.StartPointRelativePosition = 0.5;
                    _model.StartPointDocking = ConnectorUtilities.ComputeDocking(_model.From.Bounds, 
                                                    _model.StartPointRelativePosition, ref _start);

                }
                else
                {
                    _start = ConnectorUtilities.ComputePoint(_model.From.Bounds,
                                                _model.StartPointDocking, _model.StartPointRelativePosition);
                }

                if (_model.EndPointDocking == ConnectorDocking.Undefined)
                {
                    _end = ConnectorUtilities.Intersect(_model.To.Bounds, toCenter, fromCenter);

                    _model.EndPointRelativePosition = 0.5;
                    _model.EndPointDocking = ConnectorUtilities.ComputeDocking(_model.To.Bounds,
                                                _model.EndPointRelativePosition, ref _end);
                }
                else
                {
                    _end = ConnectorUtilities.ComputePoint(_model.To.Bounds,
                                                _model.EndPointDocking, _model.EndPointRelativePosition);
                }

                // avoid two overlaying connections with same end points
                var connectorWithSameEndpoints = ConnectorUtilities.GetConnectorsWithSameEndpoints(
                    _model.Siblings, _model.From, _model.To, _start, _end);
                if (connectorWithSameEndpoints.Any())
                {
                    _model.StartPointRelativePosition = Math.Min(1.0, connectorWithSameEndpoints.Select<ConnectorModel, double>((x) => x.StartPointRelativePosition)
                        .Max() + 0.2);
                    _start = ConnectorUtilities.ComputePoint(_model.From.Bounds,
                                                _model.StartPointDocking, _model.StartPointRelativePosition);

                    _model.EndPointRelativePosition = Math.Min(1.0, connectorWithSameEndpoints.Select<ConnectorModel, double>((x) => x.EndPointRelativePosition)
                        .Max() + 0.2);
                    _end = ConnectorUtilities.ComputePoint(_model.To.Bounds,
                                                _model.EndPointDocking, _model.EndPointRelativePosition);
                }


                var lineType = (LineType)((int)_model.StartPointDocking << 8 | (int)_model.EndPointDocking);
                ComputeLinePointsDelegate getLinePoints = null;
                if( ComputeConnectorLine.Table.TryGetValue( lineType, out getLinePoints) )
                {
                    var linePoints = getLinePoints(_start, _end, _model.MiddlePointRelativePosition );
                    var inVector = new Vector{
                        X = linePoints.ElementAt(1).X - linePoints.ElementAt(0).X,
                        Y = linePoints.ElementAt(1).Y - linePoints.ElementAt(0).Y
                    };
                    _startAngle = ConnectorUtilities.ComputeAngle(_model.StartPointDocking, inVector);
                    var outVector = new Vector
                    {
                        X = linePoints.Last().X - linePoints.ElementAt(linePoints.Count() - 2).X,
                        Y = linePoints.Last().Y - linePoints.ElementAt(linePoints.Count() - 2).Y
                    };
                    _endAngle = ConnectorUtilities.ComputeAngle(_model.EndPointDocking, outVector);
                    var pf = ConnectorUtilities.GetPathFigureFromPoints(linePoints);
                    pf.IsFilled = false;
                    pf.IsClosed = false;
                    _myPath.Add(pf);
                    
                    return _myPath;
                }
                throw new NotSupportedException("connector cannot be drawn");
            }
        }

        public IConnectorMoveHelper StartMove( Point p)
        {
            this._movingState = new MovingState(this._model, this, p);
            return this._movingState;
        }






    }
}
