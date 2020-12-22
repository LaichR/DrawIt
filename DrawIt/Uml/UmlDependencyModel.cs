using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch;
using Sketch.Helper;
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
        static readonly double[] _dashPattern = new double[] { 4, 2 };

        public UmlDependencyModel( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            Point connectorStartHint, Point connectorEndHint,
            ISketchItemContainer container)
            : base(type, from, to, connectorStartHint, connectorEndHint, container)
        {
            StrokeDashArray = new DoubleCollection(_dashPattern);
        }

        protected UmlDependencyModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {}

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            Endings.Clear();
            Endings.Add(new Arrow
            {
                Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                Rotation = ConnectorStrategy.EndAngle
            });
        }

        protected override void Initialize()
        {
            base.Initialize();
            StrokeDashArray = new DoubleCollection(_dashPattern);
        }


    }
}
