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

namespace DrawIt.Uml
{
    [Serializable]
    public class UmlFinalStateModel : ConnectableBase
    {
        const int DefaultWidth = 26;
        const int DefaultHeight = 26;
        const int OuterRadius = 12;
        const int InnerRadius1 = 11;
        const int InnerRadius2 = 8;

        
        
        GeometryGroup _geometry;
        EllipseGeometry _inner;
        CombinedGeometry _outer;
        public UmlFinalStateModel(Point p)
        {
            var location = new Point(p.X, p.Y);
  
            Bounds = new Rect(location, new Size(DefaultHeight, DefaultWidth));
            AllowEdit = false;
            AllowSizeChange = false;
            LabelArea = Rect.Empty;
            IsSelected = true;
            AllowSizeChange = true;
            Name = "Final-State";
            RotationAngle = 0.0;
            
            _geometry = new GeometryGroup();
            FillColor = Colors.Black;
            ComputeGeometry();

        }

        protected UmlFinalStateModel(SerializationInfo info, StreamingContext context) : 
            base(info, context) 
        {
            _geometry = new GeometryGroup();
        }

        public void ComputeGeometry()
        {
            var center = new Point((Bounds.Left + Bounds.Right)/2, 
                (Bounds.Top + Bounds.Bottom)/2);

            var innerLocation1 = new Point(Bounds.TopLeft.X + 2, Bounds.TopLeft.Y + 2);
            var innerLocation2 = new Point(Bounds.TopLeft.X + 5, Bounds.TopLeft.Y + 5);
            var arcStartLocation = new Point(Bounds.TopLeft.X, (Bounds.TopLeft.Y + Bounds.BottomRight.Y) / 2);
            _inner = new EllipseGeometry(center, InnerRadius2, InnerRadius2);

            _outer = new CombinedGeometry();
            _outer.Geometry1 = new EllipseGeometry(center,
                OuterRadius, OuterRadius);

            _outer.Geometry2 = new EllipseGeometry(center,
                InnerRadius1, InnerRadius1);
            _outer.GeometryCombineMode = GeometryCombineMode.Xor;

            _geometry.Children.Clear();
            _geometry.Children.Add( _outer );
            _geometry.Children.Add(_inner);
            
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

        //public override IList<ICommandDescriptor> Tools
        //{
        //    get { return new List<UI.Utilities.Interfaces.ICommandDescriptor>(); }
        //}

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            _geometry = new GeometryGroup();
        }

    }
}
