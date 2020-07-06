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
        new const int DefaultWidth = 26;
        new const int DefaultHeight = 26;


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
            Label = "Initial-State";
            RotationAngle = 0.0;
            FillColor = Colors.Black;
            UpdateGeometry();
        }

        protected UmlInitialStateModel(SerializationInfo info, StreamingContext context) : 
            base(info, context) 
        {
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            var g = Geometry as GeometryGroup;
            g.Children.Clear();
            g.Children.Add(new EllipseGeometry(Bounds));
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

        protected override void Initialize()
        {
            base.Initialize();
        }

    }
}
