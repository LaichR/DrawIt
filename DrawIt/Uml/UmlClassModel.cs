using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Models;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using Sketch.Interface;
using System.Runtime.Serialization;


namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlDependencyModel))]
    [AllowableConnector(typeof(UmlAssociationModel))]
    [AllowableConnector(typeof(UmlCompositionModel))]
    [AllowableConnector(typeof(UmlGeneralizationModel))]
    
    public class UmlClassModel: ContainerModel
    {
        const int DefaultWidth = 150;
        const int DefaultHeight = 75;

        public UmlClassModel(Point p)
            : base(p, new Size(DefaultWidth, DefaultHeight)) 
        {
            var location = new Point(p.X, p.Y);
            LabelArea = new Rect(location, new Size(Bounds.Width, 20));
            IsSelected = true;
            AllowSizeChange = true;
            AllowEdit = true;
            Name = "new class";
            RotationAngle = 0.0;
        }

        protected UmlClassModel(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override System.Windows.Media.Geometry Geometry
        {
            get
            {
                var myGeometry = new GeometryGroup();
 
                var body = new Rect(Bounds.Left, Bounds.Top , Bounds.Width, Bounds.Height);
                myGeometry.Children.Add(new LineGeometry(LabelArea.BottomLeft, LabelArea.BottomRight));
                myGeometry.Children.Add(new RectangleGeometry(body, 2, 2));
                myGeometry.Transform = Rotation;
                return myGeometry;
            }
        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get
            {
                var outline = new System.Windows.Media.RectangleGeometry(Bounds);
                outline.Transform = Rotation;
                return outline;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
