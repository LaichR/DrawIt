using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using Sketch.Helper;
using Sketch.Models;
using System.Runtime.Serialization;
using Sketch.Interface;

namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlTransitionModel))]
    public class UmlInitialStateModel : ConnectableBase
    {
        new const int DefaultWidth = 26;
        new const int DefaultHeight = 26;


        //EllipseGeometry _geometry;

        public UmlInitialStateModel(Point p)
            :base(p, new Size(DefaultHeight, DefaultWidth), "Initial-State", Colors.Black)
        {
            CanEditLabel = false;
            CanChangeSize = false;
            LabelArea = Rect.Empty;
            IsSelected = true;
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
            g.Children.Add(new EllipseGeometry(new Rect(0,0, Bounds.Width, Bounds.Height)));
        }

        //protected override void FieldDataRestored()
        //{
        //    base.FieldDataRestored();
        //    _geometry = new EllipseGeometry(Bounds);
        //}

        protected override Rect ComputeBounds(Point pos, Size size, Rect labelArea)
        {
            return new Rect(pos, size);
        }
    }
}
