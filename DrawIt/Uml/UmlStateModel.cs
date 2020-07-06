using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using Sketch.Types;
using Sketch.Models;
using System.Runtime.Serialization;
using Sketch.Interface;

namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlTransitionModel))]
    public class UmlStateModel : ContainerModel
    {
        new static readonly double DefaultWidth = 150;
        new static readonly double DefaultHeight = 60;

        static readonly double DefaultRoundingEdgeRadius = DefaultHeight / 3; 

        public UmlStateModel(Point p)
            : base(p, new Size( DefaultWidth, DefaultHeight)) 
        {
            var location = new Point(p.X, p.Y);
            var labelAreaLocation = new Point(p.X + 10, p.Y);
            LabelArea = new Rect(labelAreaLocation, new Size(Bounds.Width-20, 25));
            IsSelected = true;
            AllowSizeChange = true;
            Label = "new state";
            RotationAngle = 0.0;
            UpdateGeometry();
        }

        protected UmlStateModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {

            var myGeometry = Geometry as GeometryGroup;
            myGeometry.Children.Clear();


            var body = new Rect(Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
            myGeometry.Children.Add(new LineGeometry(new Point(Bounds.Left, LabelArea.Bottom),
                                                      new Point(Bounds.Right, LabelArea.Bottom)));
            myGeometry.Children.Add(new RectangleGeometry(body, DefaultRoundingEdgeRadius, DefaultRoundingEdgeRadius));
            myGeometry.Transform = Rotation;
            

        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get
            {
                var outline = new System.Windows.Media.RectangleGeometry(Bounds, DefaultRoundingEdgeRadius, DefaultRoundingEdgeRadius);
                outline.Transform = Rotation;
                return outline;
            }
        }

    }
}
