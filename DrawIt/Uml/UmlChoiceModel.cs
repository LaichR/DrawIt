using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using UI.Utilities.Interfaces;
using Sketch.Helper;
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

        public UmlChoiceModel(Point p, ISketchItemContainer container )
            :base(p, container, new Size(UmlChoiceModel.DefaultWidth,
                UmlChoiceModel.DefaultHeight),
                 "Choice", Colors.Snow)
        {
            
            CanChangeSize = false;
            
        }

        protected UmlChoiceModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
            UpdateGeometry();
        }
        

        public override void UpdateGeometry()
        {
            var g = Geometry as GeometryGroup;
            g.Children.Clear();

            List<Point> border = new List<Point>
                {
                    new Point( 0, Bounds.Height/2),
                    new Point( Bounds.Width/2, 0),
                    new Point( Bounds.Width, Bounds.Height/2),
                    new Point( Bounds.Width/2, Bounds.Height),
                };
            var pf = GeometryHelper.GetPathFigureFromPoint(border);
            pf.IsClosed = true;
            pf.IsFilled = true;
            g.Children.Add(GeometryHelper.GetGeometryFromPath(pf));

        }

        protected override Rect ComputeLabelArea(string label)
        {
            return Rect.Empty;
        }

    }
}
