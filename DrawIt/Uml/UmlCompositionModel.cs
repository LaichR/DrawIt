using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;
using System.Windows.Media;
using Sketch;
using Sketch.Interface;
using Sketch.Types;
using Sketch.Models;
using Sketch.Models.Geometries;


namespace DrawIt.Uml
{
    [Serializable]
    public class UmlCompositionModel: ConnectorModel
    {
        public UmlCompositionModel(ConnectionType type, IBoundedItemModel from, IBoundedItemModel to)
            :base(type, from, to)
        {
            UpdateGeometry();
        }

        public UmlCompositionModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var geometry = Geometry as GeometryGroup;
            geometry.Children.Add(new Diamond
            {
                Translation = new Vector(ConnectorStrategy.ConnectionStart.X, ConnectorStrategy.ConnectionStart.Y),
                Rotation = ConnectorStrategy.StartAngle,
            }.Ending);

            geometry.Children.Add(new Arrow
            {
                Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                Rotation = ConnectorStrategy.EndAngle
            }.Ending);
        }

        protected override void Initialize()
        {
            base.Initialize();
            FillEndings = true;
        }

    }
}
