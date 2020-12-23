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
    [AllowableConnector(typeof(UmlMessageModel))]
    public class UmlLifeLineModel : ConnectableBase
    {
        
        new public static readonly double DefaultHeight = 350;
        new public static readonly double DefaultWidth = 36;

        static readonly Pen _lifeLinePen = new Pen(Brushes.Black, 1)
        { DashStyle = new DashStyle(new double[] { 5, 5 }, 0), DashCap = PenLineCap.Square };
            
        LineGeometry _lifeLine;


        public UmlLifeLineModel(Point p, ISketchItemContainer container )
            :base(p, container, new Size(UmlLifeLineModel.DefaultWidth,
                UmlLifeLineModel.DefaultHeight),
                 "Object Life Line", Colors.Snow)
        {
            FillColor = new Color() { A = 0, R = 0xFF, G = 0xFF, B = 0xFF };

            CanChangeSize = true;
            UpdateGeometry();
        }

        

        protected UmlLifeLineModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
            UpdateGeometry();
        }


        

        public override void UpdateGeometry()
        {
            var g = Geometry as GeometryGroup;
            g.Children.Clear();
            var r = new Rect(new Point(0,0), 
                new Point(Bounds.Width, LabelArea.Height + 10));
            var rg = new RectangleGeometry(r);
            _lifeLine = new LineGeometry(
                new Point(Bounds.Width / 2, LabelArea.Height + 10),
                new Point(Bounds.Width / 2, Bounds.Height- LabelArea.Height + 10)
                );
            
            g.Children.Add(rg);
            //g.Children.Add(_lifeLine);
        }

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(_lifeLinePen.Brush, _lifeLinePen , _lifeLine);   
        }

        protected override Point GetConnectorPositionInfo(Point hint, out double relativePos, out ConnectorDocking docking, out ulong port)
        {
            port = 0;
            var lifeLineX = Bounds.X + Bounds.Width / 2;
            Point p = new Point(lifeLineX, hint.Y);
            docking = ConnectorDocking.Right;
            relativePos = (p.Y -Bounds.Y) / Bounds.Height;
            if (hint.X < lifeLineX)
            {
                docking = ConnectorDocking.Left;
            }
            return p;
        }

        public override Point GetPreferredToolsLocation(Point curMouse, out ConnectorDocking docking)
        {
            Point p = Bounds.TopRight;
            double middle = Bounds.X + Bounds.Width / 2;
            //p.X = middle + 10;
            docking = ConnectorDocking.Right;
            if( curMouse.X < middle)
            {
                docking = ConnectorDocking.Left;
                p = Bounds.TopLeft;
            }
            p.Y = curMouse.Y-10;
            return p;
        }

        public override Point GetConnectorPoint(ConnectorDocking docking, double relativePosition, ulong connectorPort)
        {
            double y = Bounds.Height * relativePosition + Bounds.Top;
            return new Point(Bounds.Left + Bounds.Width / 2, y);
        }

        public double LifeLineX
        {
            get => Bounds.Width / 2;
        }

        public override string Label 
        { 
            get => base.Label;
            set
            {
                base.Label = value;
                AdjustBounds();
            }
        }

        void AdjustBounds()
        {
            if (Bounds.Left != 0) // the bounds where not yet initialized
            {
                LabelArea = ComputeLabelArea(DisplayedLabel());
                var w = Math.Max(DefaultWidth, LabelArea.Width + 20);
                var h = Math.Max(Bounds.Height, LabelArea.Height + 20);
                Bounds = ComputeBounds(Bounds.TopLeft, new Size(w, h), LabelArea);
            }
        }

    }
}
