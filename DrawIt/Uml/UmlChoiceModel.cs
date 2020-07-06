using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using UI.Utilities.Interfaces;
using Sketch.Types;
using Sketch.Models;
using System.Runtime.Serialization;
using Sketch.Interface;

namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlDependencyModel))]
    [AllowableConnector(typeof(UmlTransitionModel))]
    public class UmlChoiceModel : ConnectableBase
    {
        new public static readonly double DefaultHeight = 34;
        new public static readonly double DefaultWidth = 36;

        public UmlChoiceModel(Point p )
            :base(p, new Size(UmlChoiceModel.DefaultWidth,
                UmlChoiceModel.DefaultHeight),
                 "Choice", Colors.Snow)
        {
            
            AllowSizeChange = false;
            LabelArea = new Rect(p, new Size(Bounds.Width, 20));
            UpdateGeometry();
        }

        protected UmlChoiceModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
            UpdateGeometry();
        }
        

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get { return new System.Windows.Media.RectangleGeometry(Bounds); }
        }

        //public override IList<ICommandDescriptor> Tools
        //{
        //    get { return _tools; }
        //}

        public override void RenderAdornments(DrawingContext drawingContext) { }

        public override void UpdateGeometry()
        {

            var g = Geometry as GeometryGroup;
            g.Children.Clear();

            List<Point> border = new List<Point>
                {
                    new Point( Bounds.Left, Bounds.Top + Bounds.Height/2),
                    new Point( Bounds.Left + Bounds.Width/2, Bounds.Top),
                    new Point( Bounds.Right, Bounds.Top + Bounds.Height/2),
                    new Point( Bounds.Left + Bounds.Width/2, Bounds.Bottom),
                };
            var pf = GeometryHelper.GetPathFigureFromPoint(border);
            pf.IsClosed = true;
            g.Children.Add(GeometryHelper.GetGeometryFromPath(pf));

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

    }
}
