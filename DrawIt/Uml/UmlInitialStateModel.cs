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
    public class UmlInitialStateModel : ContainerModel
    {
        const int DefaultWidth = 26;
        const int DefaultHeight = 26;


        EllipseGeometry _geometry;

        public UmlInitialStateModel(Point p)
            :base(p, new Size(DefaultHeight, DefaultWidth))
        {
            var location = new Point(p.X, p.Y);
            Bounds = new Rect(location, new Size(DefaultHeight, DefaultWidth));
            AllowEdit = false;
            AllowSizeChange = false;
            LabelArea = Rect.Empty;
            IsSelected = true;
            AllowSizeChange = true;
            Name = "Initial-State";
            RotationAngle = 0.0;
            FillColor = Colors.Black;
            _geometry = new EllipseGeometry(Bounds);
            

        }

        protected UmlInitialStateModel(SerializationInfo info, StreamingContext context) : 
            base(info, context) 
        {
            _geometry = new EllipseGeometry(Bounds);
        }

        public void ComputeGeometry()
        {
            _geometry.Center = new Point(
                (Bounds.Left + Bounds.Right) / 2,
                (Bounds.Top + Bounds.Bottom) / 2);
            _geometry.RadiusX = Bounds.Width / 2 -1;
            _geometry.RadiusY = _geometry.RadiusX;
        }

        public override System.Windows.Media.Geometry Geometry
        {
            get
            {
                ComputeGeometry();
                return _geometry.Clone();
            }
        }

        public override RectangleGeometry Outline
        {
            get
            {
                return new RectangleGeometry(Bounds);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            _geometry = new EllipseGeometry(Bounds);
        }

    }
}
