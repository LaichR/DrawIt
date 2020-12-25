using Sketch.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Sketch.Helper;

namespace Sketch.Models
{
    [Serializable]
    public class Waypoint : IWaypoint
    {
        [PersistentField((int)ModelVersion.V_2_1, "IncomingDocking")]
        ConnectorDocking _incomingDocking;

        [PersistentField((int)ModelVersion.V_2_1, "IncomingRelativePosition", true)]
        double _incomingRelativePosition = 0.5;

        [PersistentField((int)ModelVersion.V_2_1, "OutgoingDocking")]
        ConnectorDocking _outgoingDocking;

        [PersistentField((int)ModelVersion.V_2_1, "OutgoingRelativePosition", true)]
        double _outgoingRelativePosition=0.5;

        [PersistentField((int)ModelVersion.V_2_1, "MiddlePosition", true)]
        double _middlePosition=0.5;

        [PersistentField((int)ModelVersion.V_2_1, "Bounds")]
        Rect _bounds = new Rect(0, 0, 12, 12);

        static readonly List<FieldInfo> _persistenFields = new List<FieldInfo>();

        public Waypoint(Point p)
        {
            _bounds.X = p.X + Bounds.Width/2;
            _bounds.Y = p.Y + Bounds.Height/2;
        }

        public Point Position
        {
            get => ConnectorUtilities.ComputeCenter(_bounds);
        }

        protected Waypoint(SerializationInfo info, StreamingContext context)
        {
            PersistencyHelper.RestorePersistentFields(this, info, PersistentFields, (int)ModelVersion.V_2_1);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            PersistencyHelper.BackupPersistentFields(this, info, PersistentFields);
        }

        public ConnectorDocking IncomingDocking 
        { get => _incomingDocking; set => _incomingDocking = value; }
        public ConnectorDocking OutgoingDocking 
        { get => _outgoingDocking; set => _outgoingDocking = value; }

        public double IncomingRelativePosition 
        { get => _incomingRelativePosition; set => _incomingRelativePosition = value; }
        public double OutgoingRelativePosition 
        { get => _outgoingRelativePosition; set => _outgoingRelativePosition = value; }
        public double MiddlePosition 
        { 
          get => _middlePosition; 
          set => _middlePosition = value; 
        }

        public Rect Bounds => _bounds;

        public event EventHandler<OutlineChangedEventArgs> ShapeChanged;
        

        public ConnectorDocking AllowableDockings(bool incoming)
        {
            var docking = ConnectorDocking.Bottom | ConnectorDocking.Left | ConnectorDocking.Right | ConnectorDocking.Top;
            var complement = _incomingDocking;
            if( incoming )
            {
                complement = _outgoingDocking;
            }
            docking &= ~complement;
            return docking;
        }

        public ConnectorDocking GetConnectorDocking(Point p, bool isIncoming)
        {
            var docking = _outgoingDocking;
            if (isIncoming)
            {
                docking = _incomingDocking;
            }
            return docking;
        }

        public Point GetConnectorPoint(ConnectorDocking docking, double relativePosition, ulong connectorPort)
        {

            return ConnectorUtilities.ComputeCenter(_bounds);
        }

        public Point GetPreferredConnectorEnd(Point hint, out double relativePosition, out ConnectorDocking docking, out ulong connectorPort)
        {
            var p = GetPreferredConnectorPos(hint, out relativePosition, out _incomingDocking, out connectorPort);
            docking = _incomingDocking;
            return p;
        }

        public Point GetPreferredConnectorStart(Point hint, out double relativePosition, out ConnectorDocking docking, out ulong connectorPort)
        {
            var p = GetPreferredConnectorPos(hint, out relativePosition, out _outgoingDocking, out connectorPort);
            docking = _outgoingDocking;
            return p;
        }

        Point GetPreferredConnectorPos(Point hint, out double relativePosition, out ConnectorDocking docking, out ulong connectorPort)
        {
            relativePosition = 0.5;
            connectorPort = 0;
            if (_incomingDocking == ConnectorDocking.Undefined &&
                _outgoingDocking == ConnectorDocking.Undefined)
            {
                if (hint.X < _bounds.Left) docking = ConnectorDocking.Left;
                else if (hint.X > _bounds.Right) docking = ConnectorDocking.Right;
                else if (hint.Y < _bounds.Top) docking = ConnectorDocking.Top;
                else if (hint.Y > _bounds.Bottom) docking = ConnectorDocking.Bottom;
                else docking = ConnectorDocking.Undefined;
            }
            else if( _incomingDocking != ConnectorDocking.Undefined)
            {
                docking = OtherDocking(_incomingDocking);
            }
            else
            {
                docking = OtherDocking(_outgoingDocking);
            }
            return ConnectorUtilities.ComputeCenter(_bounds);
        }

        public void Move(Transform translation)
        {
            Rect oldRect = _bounds;
            var location = translation.Transform(_bounds.Location);
            // make sure that the middle is aligned to the grid!
            location = PlacementHelper.RoundToGrid(location);
            _bounds.Location = location;
            ShapeChanged?.Invoke(this, new OutlineChangedEventArgs(oldRect, _bounds));
        }


        IEnumerable<FieldInfo> PersistentFields
        {
            get
            {
                if( !_persistenFields.Any())
                {
                    _persistenFields.AddRange(PersistencyHelper.GetAllPersistentFields(this, typeof(object)));
                }
                return _persistenFields;
            }
        }

        ConnectorDocking OtherDocking(ConnectorDocking docking)
        {
            switch(docking)
            {
                case ConnectorDocking.Top:
                    return ConnectorDocking.Bottom;
                case ConnectorDocking.Bottom:
                    return ConnectorDocking.Top;
                case ConnectorDocking.Left:
                    return ConnectorDocking.Right;
                case ConnectorDocking.Right:
                    return ConnectorDocking.Left;
                default:
                    throw new NotSupportedException();

            }
        }

    }
}
