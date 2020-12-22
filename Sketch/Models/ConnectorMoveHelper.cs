using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sketch.Interface;
using Sketch.Helper;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Security.Cryptography;

namespace Sketch.Models
{
    class ConnectorMoveHelper : IConnectorMoveHelper
    {
        readonly MoveType _moveType;
        readonly ConnectorModel _model;
        readonly int _pathIndex = 0;
        readonly IConnectorStrategy _routingStrategy;
        readonly IWaypoint _startingFrom;
        readonly IWaypoint _endingAt;
        readonly Point _startPoint;
        readonly Point _endPoint;
        readonly double _startPointRelativePosition;
        readonly double _endPointRelativePosition;

        readonly double _distance;
        //readonly GeometryGroup _geometry = new GeometryGroup();

        public ConnectorMoveHelper(ConnectorModel connectorModel, IConnectorStrategy parent, Point p)
        {
            _routingStrategy = parent;
            _model = connectorModel;
            _moveType = MoveType.MoveTypeNone;
            _startingFrom = new ToWaypointAdapter( _model, connectorModel.From);
            _endingAt = new ToWaypointAdapter(_model, connectorModel.To);
            List<double> middlePointInfo = new List<double>() { _model.MiddlePointRelativePosition };
            List<IWaypoint> connectionPoints = new List<IWaypoint> { _startingFrom };
            
            connectionPoints.AddRange(_model.Waypoints);
            connectionPoints.Add(_endingAt);
            
            double[] relativePositions = new double[connectionPoints.Count];
            for (int i = 0; i < relativePositions.Length; i++) relativePositions[i] = 0.5;
            relativePositions[0] = _model.StartPointRelativePosition;
            relativePositions[relativePositions.Length - 1] = _model.EndPointRelativePosition;

            middlePointInfo.AddRange(_model.Waypoints.Select<IWaypoint,double>((x)=>x.MiddlePosition));

            foreach (var pf in _routingStrategy.ConnectorPath)
            {
                var nrOfSegments = pf.Segments.Count();
                if ( ConnectorUtilities.HitLineSegment(p, pf, out int index, out _startPoint, out _endPoint))
                {
                    _distance = middlePointInfo[_pathIndex];
                    _startPointRelativePosition = relativePositions[_pathIndex];
                    _endPointRelativePosition = relativePositions[_pathIndex + 1];
                    if (index == 0 && nrOfSegments == 1)
                    {
                        var d1 = Point.Subtract(p, _startPoint).Length;
                        var d2 = Point.Subtract(p, _endPoint).Length;
                        _moveType = MoveType.MoveStartPoint;
                        if( d1 > d2)
                        {
                            _moveType = MoveType.MoveEndPoint;
                            //_moveStart = _startPoint;
                        }
                    }
                    else if( index == 0 && nrOfSegments>1)
                    {
                        _moveType = MoveType.MoveStartPoint;
                        //_moveStart = StartPoint;
                    }
                    else if ((index == 1 && nrOfSegments == 3)||(index ==2 && nrOfSegments==7)||(index==3))
                    {
                        _moveType = MoveType.MoveMiddlePoint;
                    }
                    
                    else if (index == 2 || (index == 1 && nrOfSegments == 2)||(index == 6))
                    {
                        _moveType = MoveType.MoveEndPoint;
                        _startPoint = _endPoint;
                    }

                    _startingFrom = connectionPoints[_pathIndex];
                    _endingAt = connectionPoints[_pathIndex + 1];
                    break;
                }
                _pathIndex++;
            }
        }

        public MoveType MoveType
        {
            get { return _moveType; }
        }

        public Geometry GetGeometry(GeometryGroup gg, LineType lineType, Point start, Point end, double distance)
        {
            
            //var endDocking = (int)lineType & 0xFF;
            var linePoints = _routingStrategy.ComputeLinePoints(start, end, lineType, distance, out double _0, out double _1);
            var lineStart = linePoints.First();
            
            gg.Children.Clear();
            foreach( var p in linePoints.Skip(1))
            {
                gg.Children.Add(new LineGeometry(lineStart, p));
                lineStart = p;
            }
            return gg;
        }

        public void Commit(ConnectorDocking movePointDocking, ConnectorDocking otherPointDocking, Point newMovePointPosition, Point newOtherPointPosition, double newDistance)
        {

            
            if (_moveType == MoveType.MoveStartPoint)
            {
                _startingFrom.OutgoingDocking = movePointDocking;
                _endingAt.IncomingDocking = otherPointDocking;
                if( movePointDocking != ConnectorDocking.Undefined)
                {
                    _startingFrom.OutgoingRelativePosition = ConnectorUtilities.ComputeRelativePosition(_startingFrom.Bounds, newMovePointPosition, movePointDocking);
                }
                else
                {
                    _startingFrom.OutgoingRelativePosition = 0.5;
                    _endingAt.IncomingRelativePosition = 0.5;
                    _endingAt.MiddlePosition = 0.5;
                }
                //_model.EndPointRelativePosition = ConnectorUtilities.ComputeRelativePosition(_model.To.Bounds, newOtherPointPosition, _model.EndPointDocking);
            }
            else if (MoveType == MoveType.MoveMiddlePoint)
            {
                _startingFrom.MiddlePosition = newDistance;
            }
            else if (MoveType == MoveType.MoveEndPoint)
            {
                _startingFrom.OutgoingDocking = otherPointDocking;
                _endingAt.IncomingDocking = movePointDocking;
                if (movePointDocking != ConnectorDocking.Undefined)
                {
                    //_model.StartPointRelativePosition = ConnectorUtilities.ComputeRelativePosition(_model.From.Bounds, newOtherPointPosition, _model.StartPointDocking);
                    _endingAt.IncomingRelativePosition = ConnectorUtilities.ComputeRelativePosition(_endingAt.Bounds, newMovePointPosition, movePointDocking);
                }
                else 
                {
                    _startingFrom.OutgoingRelativePosition = 0.5;
                    _endingAt.IncomingRelativePosition = 0.5;
                    _endingAt.MiddlePosition = 0.5;
                }
            }
        }

        public Point StartPoint
        {
            get => _startPoint;
        }

        public Point EndPoint
        {
            get => _endPoint;
        }

        public IWaypoint StartingFrom => _startingFrom;
        public IWaypoint EndingAt => _endingAt;


        public LineType LineType
        {
            get
            {
                LineType lt = (LineType)((int)StartingFrom.OutgoingDocking << 8 | (int)EndingAt.IncomingDocking);
                return lt;
            }
        }

        public double StartPointRelativePosition
        {
            get => _startPointRelativePosition;
        }

        public double EndPointRelativePosition
        {
            get => _endPointRelativePosition;
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
}
