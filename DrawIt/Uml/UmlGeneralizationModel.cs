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
    [AllowableConnectorTarget(typeof(UmlClassModel))]
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
            var ending = new Triangle
            {
                Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
                Rotation = ConnectorStrategy.EndAngle,
                IsFilled = true
            };
            var geometry = Geometry as GeometryGroup;
            Endings.Clear();
            Endings.Add(ending);

        }

        //public override void RenderAdornments(DrawingContext drawingContext)
        //{
        //    base.RenderAdornments(drawingContext);
        //    drawingContext.DrawGeometry(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), new Pen(Brushes.Black, 1),
        //        new Triangle
        //        {
        //            Translation = new Vector(ConnectorStrategy.ConnectionEnd.X, ConnectorStrategy.ConnectionEnd.Y),
        //            Rotation = ConnectorStrategy.EndAngle,
        //            IsFilled = true
        //        }.Ending);
        //}

        protected override void Initialize()
        {
            base.Initialize();
            FillEndings = false;
        }


    }
}
