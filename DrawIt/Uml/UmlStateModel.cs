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
        const int DefaultWidth = 150;
        const int DefaultHeight = 75;

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
            myGeometry.Children.Add(new RectangleGeometry(body, Bounds.Height / 4, Bounds.Height / 4));
            myGeometry.Transform = Rotation;
            

        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get
            {
                var outline = new System.Windows.Media.RectangleGeometry(Bounds, Bounds.Height/4, Bounds.Height/4);
                outline.Transform = Rotation;
                return outline;
            }
        }

    }
}
