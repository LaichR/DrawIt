using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;
using Sketch.Interface;
using Sketch.Models;
using System.Windows;
using System.Runtime.Serialization;

namespace DrawIt.Uml
{
    public class UmlActionConnector : ConnectableBase
    {
        new public const double DefaultHeight = 28;
        new public const double DefaultWidth = 28;

        FormattedText _formattedLabel;

        public UmlActionConnector(Point p)
            : base(p, new Size(DefaultHeight, DefaultWidth), "A",
                 Colors.Black)
        {
            AllowEdit = true;
            AllowSizeChange = false;
            UpdateGeometry();
        }

        protected UmlActionConnector(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            UpdateGeometry();
        }

        public override string Label {
            get => base.Label;
            set
            {
                base.Label = value;
                _formattedLabel = ComputeFormattedText(Label,
                    ConnectableBase.DefaultFont,
                    ConnectableBase.DefaultFontSize);
                UpdateGeometry();
            }
        }

        public override void RenderAdornments(DrawingContext drawingContext)
        {
            drawingContext.DrawText(_formattedLabel,
                LabelArea.TopLeft);
        }

        public override void UpdateGeometry()
        {
         
            var g = Geometry as GeometryGroup;

            g.Children.Clear();
            g.Children.Add(new RectangleGeometry(
                Bounds, DefaultHeight/2, DefaultHeight/2));
        }

        public override RectangleGeometry Outline
        {
            get
            {
                return new RectangleGeometry(Bounds);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            _formattedLabel = ComputeFormattedText(
                Label, 
                ConnectableBase.DefaultFont,
                ConnectableBase.DefaultFontSize);
        }

    }
}
