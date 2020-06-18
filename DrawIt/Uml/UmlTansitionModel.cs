using System;
using System.Windows;
using System.Windows.Media;
using Sketch.Types;
using Sketch.Models;
using Sketch.Interface;
using Sketch.Models.Geometries;
using System.Runtime.Serialization;


namespace DrawIt.Uml
{
    [Serializable]

    [AllowableConnectorTarget(typeof(UmlFinalStateModel))]
    [AllowableConnectorTarget(typeof(UmlStateModel))]
    [AllowableConnectorTarget(typeof(UmlChoiceModel))]
    public class UmlTransitionModel: ConnectorModel
    {
        public UmlTransitionModel( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to)
            :base(type, from, to)
        {}



        protected UmlTransitionModel(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected override void ComputeGeometry()
        {
            base.ComputeGeometry();
            var g = Geometry as GeometryGroup;
            g.Children.Add(new Arrow
            {
                Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                Rotation = ConnectorStrategy.EndAngle
            }.Ending);
        }

    }
}
