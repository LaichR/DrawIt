using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch;
using Sketch.Types;
using Sketch.Models;
using Sketch.Interface;
using Sketch.Models.Geometries;
using System.IO;
using System.Runtime.Serialization;


namespace DrawIt.Uml
{
    [Serializable]
    public class UmlGeneralizationModel: ConnectorModel
    {
        public UmlGeneralizationModel( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            Point connectorStartHint, Point connectorEndHint,
            ISketchItemContainer container)
            : base(type, from, to, connectorStartHint, connectorEndHint, container)
        { }

        protected UmlGeneralizationModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {}

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var geometry = Geometry as GeometryGroup;
            geometry.Children.Add(new Triangle
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
