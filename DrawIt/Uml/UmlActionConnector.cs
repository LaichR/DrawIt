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
    [Serializable]
    [AllowableConnector(typeof(UmlTransitionModel))]
    public class UmlActionConnector : ConnectableBase
    {
        new public const double DefaultHeight = 30;
        new public const double DefaultWidth = 30;

        FormattedText _formattedLabel;
        Point _textPosition = new Point();

        public UmlActionConnector(Point p, ISketchItemContainer container)
            : base(p, container, new Size(DefaultHeight, DefaultWidth), "A",
                 Colors.White)
        {
            CanEditLabel = true;
            CanChangeSize = false;
            StrokeThickness = 3;
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
                _textPosition.Y = (Bounds.Height) / 2.0 - _formattedLabel.Height / 2.0;
                _textPosition.X = (LabelArea.Width) / 2.0 - _formattedLabel.Width / 2.0;
                UpdateGeometry();
            }
        }

        //public override void RenderAdornments(DrawingContext drawingContext)
        //{
            
        //    drawingContext.DrawText(_formattedLabel, _textPosition
        //        );
        //}

        public override void UpdateGeometry()
        {
         
            var g = Geometry as GeometryGroup;

            g.Children.Clear();
            var r = new Rect(0, 0, Bounds.Width, Bounds.Height);
            //r.X += 1; r.Y += 1; 
            //if( r.Width > 0 ) r.Width -= 2; 
            //if( r.Height > 0 ) r.Height -= 2;
            g.Children.Add(new RectangleGeometry( 
                r, (r.Width)/2 + 1.0, r.Width/ 2 + 1.0));
        }


        protected override void Initialize()
        {
            base.Initialize();
            _formattedLabel = ComputeFormattedText(
                Label, 
                ConnectableBase.DefaultFont,
                ConnectableBase.DefaultFontSize);
            _textPosition.Y = (Bounds.Top + Bounds.Bottom) / 2.0 - _formattedLabel.Height / 2;
            _textPosition.X = (LabelArea.Left + LabelArea.Right) / 2.0 - _formattedLabel.Width / 2.0;
        }

    }
}
