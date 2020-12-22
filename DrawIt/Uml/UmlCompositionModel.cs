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
using Sketch.Helper;
using Sketch.Models;
using Sketch.Models.Geometries;
using System.ComponentModel;

namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnectorTarget(typeof(UmlClassModel))]
    public class UmlCompositionModel: ConnectorModel
    {

        [PersistentField((int)ModelVersion.V_2_2, nameof(IsDirected), true)]
        bool _isDirected = true;

        public UmlCompositionModel(ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            Point connectorStartHint, Point connectorEndHint,
            ISketchItemContainer container)
            : base(type, from, to, connectorStartHint, connectorEndHint, container)
        {
            UpdateGeometry();
        }

        public UmlCompositionModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            UpdateGeometry();
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

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var geometry = Geometry as GeometryGroup;
            geometry.Children.Add(new Diamond
            {
                Translation = new Vector(ConnectorStrategy.ConnectionStart.X, ConnectorStrategy.ConnectionStart.Y),
                Rotation = ConnectorStrategy.StartAngle,
            }.Ending);

            if (_isDirected)
            {
                geometry.Children.Add(new Arrow
                {
                    Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                    Rotation = ConnectorStrategy.EndAngle
                }.Ending);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            FillEndings = true;
        }

    }
}
