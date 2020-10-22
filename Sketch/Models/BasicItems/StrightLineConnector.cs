using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Interface;
using Sketch.Models;
using Sketch.Models.Geometries;

namespace Sketch.Models.BasicItems
{
    [Serializable]
    public class StrightLineConnector : ConnectorModel
        
    {
        public StrightLineConnector(ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            Point connectorStartHint, Point connectorEndHint,
            ISketchItemContainer container)
            : base(ConnectionType.StrightLine, from, to, connectorStartHint, connectorEndHint, container)
        {
            UpdateGeometry();
        }
        protected StrightLineConnector(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var g = Geometry as GeometryGroup;
            g.Children.Add(new Arrow
            {
                Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                Rotation = ConnectorStrategy.EndAngle
            }.Ending);
        }
    }
}
