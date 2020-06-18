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
        IList<UI.Utilities.Interfaces.ICommandDescriptor> _tools;
        

        public UmlChoiceModel(Point p )
        {
            Bounds = new Rect(p, new Size(36, 34));
            IsSelected = true;
            AllowSizeChange = false;
            Name = "Choice";
            RotationAngle = 0.0;
            LabelArea = new Rect(p, new Size(Bounds.Width, 20));
            FillColor = System.Windows.Media.Colors.Snow;
        }

        protected UmlChoiceModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {}
        

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get { return new System.Windows.Media.RectangleGeometry(Bounds); }
        }

        //public override IList<ICommandDescriptor> Tools
        //{
        //    get { return _tools; }
        //}

        public override void RenderAdornments(DrawingContext drawingContext) { }

        public override System.Windows.Media.Geometry Geometry
        {
            get {
                List<Point> border = new List<Point>
                {
                    new Point( Bounds.Left, Bounds.Top + Bounds.Height/2),
                    new Point( Bounds.Left + Bounds.Width/2, Bounds.Top),
                    new Point( Bounds.Right, Bounds.Top + Bounds.Height/2),
                    new Point( Bounds.Left + Bounds.Width/2, Bounds.Bottom),
                };
                var pf = GeometryHelper.GetPathFigureFromPoint(border);
                pf.IsClosed = true;
                return GeometryHelper.GetGeometryFromPath(pf);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

    }
}
