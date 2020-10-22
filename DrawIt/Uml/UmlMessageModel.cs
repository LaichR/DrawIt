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
    
    [AllowableConnectorTarget(typeof(UmlLifeLineModel))]
    public class UmlMessageModel: ConnectorModel
    {

       

        public UmlMessageModel( ConnectionType type, IBoundedItemModel from, IBoundedItemModel to,
            Point connectorStartHint, Point connectorEndHint,
            ISketchItemContainer container)
            : base(ConnectionType.StrightLine, from, to, connectorStartHint, connectorEndHint, container)
        {
            Label = "message()";
            UpdateGeometry();
        }


        public override void RenderAdornments(DrawingContext drawingContext)
        { }


        public override Rect GetLabelArea(bool isStartPointLabel)
        {
            var area = base.GetLabelArea(true);
            var fromMiddle = From.Bounds.Left + From.Bounds.Width / 2;
            var toMiddle = To.Bounds.Left + To.Bounds.Width / 2;
            var x = fromMiddle / 2 + toMiddle / 2;
            area.X = x - area.Width / 2;
            area.Y = ConnectorStrategy.ConnectionStart.Y + area.Height / 2;
            return area;
        }

        


        protected UmlMessageModel(SerializationInfo info, StreamingContext context) : base(info, context) { }

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

        

        public override void ShowConnectorLabel(Point p, bool isConnectorStartLabel)
        {
            base.ShowConnectorLabel(p, true);
        }

        public override void Move(Transform translation)
        {
            if (!(ConnectorStartSelected || ConnectorStartSelected))
            {
                ConnectorStartSelected = true;
                ConnectorEndSelected = true;
                base.Move(translation);
                ConnectorStartSelected = false;
                ConnectorEndSelected = false;
            }
            else
            {
                base.Move(translation);
            }
        }

        protected override bool CanExecuteShowLabel()
        {
            return true;
        }

        protected override bool ShowLableHaderConnection => false;

        protected override Point GetPositionFromDocking(bool isStartPointLabel)
        {
            var fromMiddle = From.Bounds.Left + From.Bounds.Width / 2;
            var toMiddle = To.Bounds.Left + To.Bounds.Width / 2;

            var x = From.Bounds.Right;
            if( fromMiddle > toMiddle)
            {
                x = From.Bounds.Left-100;
            }

            var y = ConnectorStrategy.ConnectionStart.Y - 30;
            return new Point(x, y);
        }
    }
}
