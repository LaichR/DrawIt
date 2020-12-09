using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Sketch.Interface
{
    public interface IConnectable : IBoundsProvider, ISerializable
    {
        /// <summary>
        /// Some Sketch elements may require, that incoming connections are always docked to the left side
        /// and outgoing connection to the right side
        /// Other shapes may have no restrictions. This method is used to query this information
        /// </summary>
        /// <param name="incoming">indicates if this information is related
        /// to incoming or outgoing connections
        /// </param>
        /// <returns>Bitmap containing the queried constraints</returns>
        ConnectorDocking AllowableDockings(bool incoming);

        Point GetPreferredConnectorStart(Point hint, out double relativePosition, out ConnectorDocking docking, out ulong connectorPort);

        Point GetPreferredConnectorEnd(Point hint, out double relativePosition, out ConnectorDocking docking, out ulong connectorPort);

        Point GetConnectorPoint(ConnectorDocking docking, double relativePosition, ulong connectorPort);

        ConnectorDocking GetConnectorDocking(Point position, bool incomingConnection);

        event EventHandler<OutlineChangedEventArgs> ShapeChanged;

    }
}
