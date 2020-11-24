using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Sketch.Interface;
using Sketch.Models;
using Sketch.Models.Geometries;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace DrawIt.Uml
{
    [Serializable]
    
    [AllowableConnectorTarget(typeof(UmlClassModel))]
    [AllowableConnectorTarget(typeof(UmlPackageModel))]
    public class UmlAssociationModel: ConnectorModel
    {

        [PersistentField((int)ModelVersion.V_2_0, nameof(IsDirected), true)]
        bool _isDirected = true;

        public UmlAssociationModel( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            Point connectorStartHint, Point connectorEndHint,
            ISketchItemContainer container)
            : base(type, from, to, connectorStartHint, connectorEndHint, container)
        {
            UpdateGeometry();
        }



        protected UmlAssociationModel(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var g = Geometry as GeometryGroup;
            if (IsDirected)
            {
                g.Children.Add(new Arrow
                {
                    Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                    Rotation = ConnectorStrategy.EndAngle
                }.Ending);
            }
        }

        [Browsable(true)]
        public bool IsDirected
        {
            get => _isDirected;
            set
            {
                SetProperty<bool>(ref _isDirected, value);
                UpdateGeometry();
            }
        }

    }
}
