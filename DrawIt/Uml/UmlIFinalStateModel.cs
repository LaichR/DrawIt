
using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using Sketch.Models;
using Sketch.Interface;

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
        public UmlFinalStateModel(Point p, ISketchItemContainer container)
            : base(p, container, new Size(DefaultHeight, DefaultWidth), "Final-State",
                 Colors.Black)
        {
            CanEditLabel = false;
            CanChangeSize = false;
            LabelArea = Rect.Empty;
        }

        protected UmlFinalStateModel(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            var center = new Point((Bounds.Width) / 2,
                (Bounds.Height) / 2);

            _inner = new EllipseGeometry(center, InnerRadius2, InnerRadius2);

            _outer = new CombinedGeometry()
            {
                Geometry1 = new EllipseGeometry(center, OuterRadius, OuterRadius),
                Geometry2 = new EllipseGeometry(center, InnerRadius1, InnerRadius1),
                GeometryCombineMode = GeometryCombineMode.Xor
            };

            var g = Geometry as GeometryGroup;

            g.Children.Clear();
            g.Children.Add(_outer);
            g.Children.Add(_inner);

        }


        protected override Rect ComputeBounds(Point pos, Size size, Rect labelArea)
        {
            return new Rect(pos, size);
        }

    }
}
