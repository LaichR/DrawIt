using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Interface;
using Sketch.Types;

namespace Sketch.Models
{
    public interface IConnectorStrategy
    {

        ConnectionType ConnectionType
        {
            get;
        }

        bool AllowWaypoints
        {
            get;
        }

        Point ConnectionStart
        {
            get;
        }

        Point ConnectionEnd
        {
            get;
        }

        double StartAngle
        {
            get;
        }

        double EndAngle
        {
            get;
        }

        IEnumerable<PathFigure> ConnectorPath
        {
            get;
        }

        IConnectorMoveHelper StartMove( Point p);

        IEnumerable<Point> ComputeLinePoints(Point start, Point end, LineType lineType, double middlePosition, out double startAngle, out double endAngle);


        //void ComputeDockingDuringMove(Rect rect, Point p, ref ConnectorDocking currentDocking, ref Point lastPos);


    }


}
