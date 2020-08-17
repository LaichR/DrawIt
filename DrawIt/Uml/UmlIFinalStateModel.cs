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
        new const double DefaultWidth = 28;
        new const double DefaultHeight = 28;
        const int OuterRadius = 14;
        const int InnerRadius1 = 13;
        const int InnerRadius2 = 9;

       
        EllipseGeometry _inner;
        CombinedGeometry _outer;
        public UmlFinalStateModel(Point p)
            :base(p, new Size(DefaultHeight, DefaultWidth), "Final-State",
                 Colors.Black)
        {
            AllowEdit = false;
            AllowSizeChange = false;
            LabelArea = Rect.Empty;
        }

        protected UmlFinalStateModel(SerializationInfo info, StreamingContext context) : 
            base(info, context) 
        {
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            var center = new Point((Bounds.Width)/2, 
                (Bounds.Height)/2);

            var innerLocation1 = new Point(2, 2);
            var innerLocation2 = new Point(5, 5);
            
            _inner = new EllipseGeometry(center, InnerRadius2, InnerRadius2);

            _outer = new CombinedGeometry();
            _outer.Geometry1 = new EllipseGeometry(center,
                OuterRadius, OuterRadius);

            _outer.Geometry2 = new EllipseGeometry(center,
                InnerRadius1, InnerRadius1);
            _outer.GeometryCombineMode = GeometryCombineMode.Xor;

            var g = Geometry as GeometryGroup;

            g.Children.Clear();
            g.Children.Add( _outer );
            g.Children.Add(_inner);
            
        }


        protected override Rect ComputeBounds(Point pos, Size size, Rect labelArea)
        {
            return new Rect(pos, size);
        }

    }
}
