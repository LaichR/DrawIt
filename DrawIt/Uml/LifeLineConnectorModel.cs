using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Sketch.Models;
using System.Runtime.Serialization;
using Sketch.Models.Geometries;

namespace Drawing.Uml
{
    [Serializable]
    public class LifeLineConnectorModel:ConnectorModel
    {
        public LifeLineConnectorModel(UmlLifeLineModel from, UmlLifeLineModel to)
            : base(Sketch.Types.ConnectionType.StrightLine, from, to)
        {
            if( from.Bounds.Left > to.Bounds.Left)
            {
                StartPointDocking = Sketch.Types.ConnectorDocking.Left;
                EndPointDocking = Sketch.Types.ConnectorDocking.Right;
                StartPointRelativePosition = from.GetNextInteractionPosition();

            }
            else
            {
                StartPointDocking = Sketch.Types.ConnectorDocking.Right;
                EndPointDocking = Sketch.Types.ConnectorDocking.Left;
            }
        }


        protected LifeLineConnectorModel(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override System.Windows.Media.Geometry Geometry
        {
            get
            {
                var path = ConnectorStrategy.ConnectorPath;
                var geometry = new GeometryGroup();
                geometry.Children.Add(new PathGeometry(path));
                geometry.Children.Add(new Arrow
                {
                    Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                    Rotation = ConnectorStrategy.EndAngle
                }.Ending);

                return geometry;
            }
        }


    }
}
