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
        new const int DefaultWidth = 150;
        new const int DefaultHeight = 75;

        public UmlClassModel(Point p)
            : base(p, new Size(DefaultWidth, DefaultHeight)) 
        {
            AllowSizeChange = true;
            AllowEdit = true;
            Label = "new class";
            RotationAngle = 0.0;
            //UpdateGeometry();
        }

        protected override Rect ComputeLabelArea(string label)
        {
            var location = new Point(5, 5);
            return new Rect(location, new Size(Bounds.Width, 20)); ;
        }


        protected UmlClassModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            UpdateGeometry();
        }

        public override System.Windows.Media.Geometry Geometry
        {
            get
            {
                var myGeometry = new GeometryGroup();

                var body = new Rect(0, 0, Bounds.Width, Bounds.Height);
                myGeometry.Children.Add(new RectangleGeometry(body, 2, 2));
                myGeometry.Transform = Rotation;
                return myGeometry;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
