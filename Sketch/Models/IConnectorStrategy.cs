using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Types;

namespace Sketch.Models
{
    public interface IConnectorStrategy
    {
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
        

    }


}
