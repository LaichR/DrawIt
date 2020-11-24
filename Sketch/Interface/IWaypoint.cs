using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Sketch.Interface
{
    public interface IWaypoint: IConnectable
    {
        ConnectorDocking IncomingDocking
        {
            get;
            set;
        }

        ConnectorDocking OutgoingDocking
        {
            get;
            set;
        }

        double IncomingRelativePosition
        {
            get;
            set;
        }

        double OutgoingRelativePosition
        {
            get;
            set;
        }

        double MiddlePosition
        {
            get;
            set;
        }

        void Move(Transform transform);
    }
}
