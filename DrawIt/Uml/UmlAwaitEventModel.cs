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
using System.Windows.Media.Imaging;
using UI.Utilities;
using UI.Utilities.Behaviors;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Specialized;

namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlTransitionModel))]
    public class UmlAwaitEventModel : ConnectableBase
    {
        new static readonly double DefaultWidth = 110;
        new static readonly double DefaultHeight = 40;
         

        public UmlAwaitEventModel(Point p, ISketchItemContainer container)
            : base(p, container, new Size( DefaultWidth, DefaultHeight), "await event", ConnectableBase.DefaultColor ) 
        {
            IsSelected = true;
            CanChangeSize = true;
            RotationAngle = 0.0;
            Commands = GetUmlWaitForEventCommands();
        }



        protected override Rect ComputeLabelArea(string label)
        {
            var labelAreaLocation = new Point(5 + DefaultHeight/2, 5); // the signal flag requires some space!
            var size = new Size(DefaultWidth-DefaultHeight/2 - 20, DefaultHeight / 2);
            
            if (!string.IsNullOrEmpty(label))
            {
                size = ComputeFormattedTextSize(label, ConnectableBase.DefaultFont, ConnectableBase.DefaultFontSize,
                ConnectableBase.MinimalTextMarginX, ConnectableBase.MinimalTextMarginY);

            }
          
            return new Rect(labelAreaLocation, size); // the entier shape may contain text!
        }

        protected UmlAwaitEventModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {

            var myGeometry = Geometry as GeometryGroup;
            myGeometry.Children.Clear();

            Point[] pathPoints = new []
            {
                new Point(0,0),
                new Point(Bounds.Width, 0),
                new Point(Bounds.Width, Bounds.Height),
                new Point(0, Bounds.Height),
                new Point(Bounds.Height/2, Bounds.Height/2)
            };
            var pf = ConnectorUtilities.GetPathFigureFromPoints(pathPoints);
            pf.IsClosed = true; pf.IsFilled = true;

            myGeometry.Children.Add(new PathGeometry(new[] { pf }));
               
            myGeometry.Transform = Rotation;
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



        public override System.Windows.Media.RectangleGeometry Outline
        {
            get
            {
                var outline = new System.Windows.Media.RectangleGeometry(Bounds)
                {
                    Transform = Rotation
                };
                
                return outline;
            }
        }

        

        void AdjustBounds()
        {
            if (Bounds.Left != 0) // the bounds where not yet initialized
            {
                LabelArea = ComputeLabelArea(DisplayedLabel());
                var w = Math.Max(DefaultWidth, LabelArea.Width + 20);
                var h = Math.Max(DefaultHeight, LabelArea.Height);
                
                Bounds = ComputeBounds(Bounds.TopLeft, new Size(w, h), LabelArea);
            }
        }


        

        List<ICommandDescriptor> GetUmlWaitForEventCommands()
        {
            List<ICommandDescriptor> commands = new List<ICommandDescriptor>
            {
               
            };
            return commands;
        }

    }
}
