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
    [AllowableConnectorTarget(typeof(UmlNoteModel))]
    [AllowableConnectorTarget(typeof(UmlPackageModel))]
    [AllowableConnectorTarget(typeof(UmlClassModel))]
    public class UmlDependencyModel: ConnectorModel
    {
        public UmlDependencyModel( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            ISketchItemContainer container)
            :base(type, from, to, container)
        {
            StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
        }

        protected UmlDependencyModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {}

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var geometry = Geometry as GeometryGroup;
                
            geometry.Children.Add(new Arrow
                {
                    Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                    Rotation = ConnectorStrategy.EndAngle
                }.Ending);
             
        }

        protected override void Initialize()
        {
            base.Initialize();
            StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
        }


    }
}
