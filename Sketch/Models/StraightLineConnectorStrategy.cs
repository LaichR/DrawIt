using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Sketch.Interface;
using Sketch.Types;

namespace Sketch.Models
{
    internal class StraightLineConnectorStrategy: IConnectorStrategy
    {
        
        static readonly Vector _horizontalVector = new Vector(100, 0);
        readonly ConnectorModel _model;
        Point _start;
        Point _end;


        readonly List<PathFigure> _myPath = new List<PathFigure>();

        public StraightLineConnectorStrategy(ConnectorModel connectorModel, Point start, Point end)
        {
            _model = connectorModel;
            _start = start;
            _end = end;
        }

        public ConnectionType ConnectionType => ConnectionType.StrightLine;

        public bool AllowWaypoints
        {
            get => false;
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
            if (_model.StartPointDocking == ConnectorDocking.Undefined)
            {
                _start = _model.From.GetPreferredConnectorStart(_start, out double relPos1, out ConnectorDocking docking1);
                _model.StartPointDocking = docking1;
                _model.StartPointRelativePosition = relPos1;
            }
            else
            {
                _start = _model.From.GetConnectorPoint(_model.StartPointDocking, _model.StartPointRelativePosition);
            }

            if (_model.EndPointDocking == ConnectorDocking.Undefined)
            {
                _end = _model.To.GetPreferredConnectorEnd(_end, out double relPos2, out ConnectorDocking docking2);
                _model.EndPointDocking = docking2;
                _model.EndPointRelativePosition = relPos2;
            }
            else
            {
                _end = _model.To.GetConnectorPoint(_model.EndPointDocking, _model.EndPointRelativePosition);
            }

            System.Windows.Media.PathSegmentCollection ls = new System.Windows.Media.PathSegmentCollection()
            {
                new System.Windows.Media.LineSegment(_end, true)
            };

            var pf = new System.Windows.Media.PathFigure()
            {
                StartPoint = _start,
                Segments = ls
            };

            _myPath.Add(pf);
        }


        public IConnectorMoveHelper StartMove(Point p)
        {
            return new ConnectorMoveHelper(_model, this, p);
        }

        public IEnumerable<Point> ComputeLinePoints(Point start, Point end, LineType lineType, double middlePosition, out double startAngle, out double endAngle)
        {
            ConnectorDocking startPointDocking = (ConnectorDocking)((int)lineType >> 8);
            ConnectorDocking endPointDocking = (ConnectorDocking)((int)lineType & 0xFF);
            //var pf = ConnectorUtilities.GetPathFigureFromPoints(new Point[] { start, end });

            startAngle = ConnectorUtilities.ComputeAngle(startPointDocking, end - start);
            endAngle = ConnectorUtilities.ComputeAngle(endPointDocking, end - start);
            return new[] { start, end };
        }

        public PathFigure ComputePathFigure(Point start, Point end, LineType lineType, double middlePosition, out double startAngle, out double endAngle)
        {
            return ConnectorUtilities.GetPathFigureFromPoints(
                ComputeLinePoints(start, end, lineType, middlePosition, out startAngle, out endAngle));

        }

        public void ComputeDockingDuringMove(Rect rect, Point p, ref ConnectorDocking currentDocking, ref Point lastPos)
        {
            ConnectorUtilities.ComputeDockingDuringMove(rect, p, ref currentDocking, ref lastPos);
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
