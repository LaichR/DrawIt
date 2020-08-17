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
    internal class StraightLineConnectorStrategy: IConnectorStrategy
    {
        class MovingState: IConnectorMoveHelper
        {
            StraightLineConnectorStrategy _parent;
            Point _start;
            Point _end;
            Types.MoveType _moveType;

            public MovingState(StraightLineConnectorStrategy parent, Point p)
            {
                _parent = parent;
                _start = parent.ConnectionStart;
                _end = parent.ConnectionEnd;
                var v1 = p - _start;
                var v2 = p - _end;
                _moveType = Types.MoveType.MoveStartPoint;
                
                if( (v1.Length > v2.Length))
                {
                    _moveType = Types.MoveType.MoveEndPoint; 
                }
            }

            public System.Windows.Media.Geometry GetGeometry(
                GeometryGroup gg,
                Types.LineType lineType, Point start, Point end, double distance)
            {
                
                if( _moveType == Types.MoveType.MoveStartPoint)
                {
                    _start = start;
                }
                else
                {
                    _end = end;
                }
                gg.Children.Clear();
                gg.Children.Add(new LineGeometry(_start, _end));
                return gg;
                //PathFigure path = ConnectorUtilities.GetPathFigureFromPoints(new Point[] { _start, _end }); 
                //return new PathGeometry(new PathFigure[]{path});
            }

            public void Commit(ConnectorDocking movePointDocking, 
                ConnectorDocking otherPointDocking, 
                Point newPositionStartPoint, 
                Point newPositionEndPoint, double newDistance)
            {
                //_parent._start = newPositionStartPoint;
                //_parent._end = newPositionEndPoint;
                
                if( _moveType == Types.MoveType.MoveStartPoint)
                {
                    _parent._model.StartPointDocking = movePointDocking;
                    _parent._model.EndPointDocking = otherPointDocking;
                    _parent._model.StartPointRelativePosition =
                        ConnectorUtilities.ComputeRelativePosition(_parent._model.From.Bounds, newPositionStartPoint, movePointDocking); ;
                }
                else
                {
                    _parent._model.EndPointDocking = movePointDocking;
                    _parent._model.StartPointDocking = otherPointDocking;
                    _parent._model.EndPointRelativePosition =
                            ConnectorUtilities.ComputeRelativePosition(_parent._model.To.Bounds, newPositionStartPoint, movePointDocking);
                }

            }

            public void ComputeDockingDuringMove(Rect rect, Point p, ref ConnectorDocking currentDocking, ref Point lastPos)
            {
                ConnectorUtilities.ComputeDockingDuringMove(rect, p, ref currentDocking, ref lastPos);
            }

            public Point StartPoint
            {
                get {
                    if (_moveType == Types.MoveType.MoveStartPoint)
                    {
                        return _start;
                    }
                    return _end;
                }
            }

            public ConnectionType ConnectionType
            {
                get { return ConnectionType.StrightLine ; }
            }

            public Types.LineType LineType
            {
                get { return Types.LineType.Undefined; }
            }

            public double Distance
            {
                get
                {
                    if (_moveType == Types.MoveType.MoveStartPoint)
                    {
                        return _parent._model.StartPointRelativePosition;
                    }
                    return _parent._model.EndPointRelativePosition;
                }
            }

            public Types.MoveType MoveType
            {
                get { return _moveType; }
            }
        }

        static readonly Vector _horizontalVector = new Vector(100, 0);
        ConnectorModel _model;
        Point _start;
        Point _end;

        

        List<System.Windows.Media.PathFigure> _myPath = new List<System.Windows.Media.PathFigure>();

        public StraightLineConnectorStrategy(ConnectorModel connectorModel)
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
                var fromCenter = ConnectorUtilities.ComputeCenter(_model.From.Bounds);
                var toCenter = ConnectorUtilities.ComputeCenter(_model.To.Bounds);
                _start = ConnectorUtilities.Intersect(_model.From.Bounds, fromCenter, toCenter);
                _end = ConnectorUtilities.Intersect(_model.To.Bounds, toCenter, fromCenter);

                double relPos = 0;
                _model.StartPointDocking = ConnectorUtilities.ComputeDocking(_model.From.Bounds, _start, ref relPos);
                _model.StartPointRelativePosition = relPos;

                _model.EndPointDocking = ConnectorUtilities.ComputeDocking(_model.To.Bounds, _end, ref relPos);
                _model.EndPointRelativePosition = relPos;
                
            }
            else
            {
                _start = ConnectorUtilities.ComputePoint(_model.From.Bounds, _model.StartPointDocking, _model.StartPointRelativePosition);
                _end = ConnectorUtilities.ComputePoint(_model.To.Bounds, _model.EndPointDocking, _model.EndPointRelativePosition);
            }
 

            //double relPos = 0;
            //_model.StartPointDocking = ConnectorUtilities.ComputeDocking(_model.From.Bounds, _start, ref relPos);
            //_model.StartPointRelativePosition = relPos;

            //_model.EndPointDocking = ConnectorUtilities.ComputeDocking(_model.To.Bounds, _end, ref relPos);
            //_model.EndPointRelativePosition = relPos;

            System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection();
            ls.Add(new System.Windows.Media.LineSegment(_end, true));
            var pf = new System.Windows.Media.PathFigure();
            pf.StartPoint = _start;
            pf.Segments = ls;
            _myPath.Add(pf);
        }


        public IConnectorMoveHelper StartMove(Point p)
        {
            return new MovingState(this, p);
        }


        public double StartAngle
        {
            get { return Vector.AngleBetween(_horizontalVector, Point.Subtract(_start, _end)); }
        }

        public double EndAngle
        {
            get { return Vector.AngleBetween(_horizontalVector, Point.Subtract(_end, _start)); }
        }
    }
}
