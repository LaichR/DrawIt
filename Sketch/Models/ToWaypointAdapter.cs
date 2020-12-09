using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sketch.Models
{
    class ToWaypointAdapter: IWaypoint
    {
        readonly ConnectorModel _connector;
        readonly IConnectable _connectable;
        public ToWaypointAdapter(ConnectorModel model, IConnectable connectable)
        {
            _connector = model;
            _connectable = connectable;
            _connectable.ShapeChanged += NotifyShapedChanged;
        }

        ~ToWaypointAdapter()
        {
            _connectable.ShapeChanged -= NotifyShapedChanged;
        }

        public ConnectorDocking IncomingDocking { get => _connector.EndPointDocking; set => _connector.EndPointDocking = value; }
        public ConnectorDocking OutgoingDocking { get => _connector.StartPointDocking; set => _connector.StartPointDocking = value; }
        public double MiddlePosition { get => _connector.MiddlePointRelativePosition; set => _connector.MiddlePointRelativePosition = value; }

        public Rect Bounds => _connectable.Bounds;

        public double OutgoingRelativePosition { get => _connector.StartPointRelativePosition; set => _connector.StartPointRelativePosition = value; }
        public double IncomingRelativePosition { get => _connector.EndPointRelativePosition; set => _connector.EndPointRelativePosition = value; }

        public event EventHandler<OutlineChangedEventArgs> ShapeChanged;

        public ConnectorDocking AllowableDockings(bool incoming)
        {
            return _connectable.AllowableDockings(incoming);
        }

        public ConnectorDocking GetConnectorDocking(Point position, bool incomingConnection)
        {
            var docking = _connector.StartPointDocking;
            if( incomingConnection )
            {
                docking = _connector.EndPointDocking;
            }
            return docking;
        }

        public Point GetConnectorPoint(ConnectorDocking docking, double relativePosition, ulong connectorPort)
        {
            return _connectable.GetConnectorPoint(docking, relativePosition, connectorPort);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {}

        public Point GetPreferredConnectorEnd(Point hint, out double relativePosition, out ConnectorDocking docking, out ulong connectorPort)
        {
            return _connectable.GetPreferredConnectorEnd(hint, out relativePosition, out docking, out connectorPort);
        }

        public Point GetPreferredConnectorStart(Point hint, out double relativePosition, out ConnectorDocking docking, out ulong connectorPort)
        {
            return _connectable.GetPreferredConnectorStart(hint, out relativePosition, out docking, out connectorPort);
        }

        public void Move(Transform transform)
        {
            _connector.Move(transform);
        }

        void NotifyShapedChanged(object sender, OutlineChangedEventArgs args)
        {
            ShapeChanged?.Invoke(sender, args);
        }
    }
}
