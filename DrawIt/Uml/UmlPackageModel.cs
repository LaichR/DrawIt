using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Models;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using Sketch.Interface;
using System.Runtime.Serialization;


namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlDependencyModel))]
    [AllowableConnector(typeof(UmlAssociationModel))]
    public class UmlPackageModel: ContainerModel
    {
        new const int DefaultWidth = 150;
        new const int DefaultHeight = 95;
        
        public UmlPackageModel( Point p, ISketchItemContainer container )
            : base(p, container, new Size( DefaultWidth, DefaultHeight)) 
        {
            IsSelected = true;
            CanChangeSize = true;
            Label = "new package";
            UpdateGeometry();
        }

        protected UmlPackageModel(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public override void UpdateGeometry()
        {
            
                var myGeometry = Geometry as GeometryGroup;
                myGeometry.Children.Clear();

                List<Point> points = new List<Point>
                {
                    new Point( LabelArea.Left -2, LabelArea.Top + LabelArea.Height  ),
                    new Point( LabelArea.Left, LabelArea.Top + LabelArea.Height - 2 ),
                    new Point( LabelArea.Left + 2, LabelArea.Top + 2 ),
                    new Point( LabelArea.Left+4, LabelArea.Top ),
                    new Point( LabelArea.Right + 2, LabelArea.Top ),
                    new Point( LabelArea.Right + 4, LabelArea.Top + 1 ),
                    new Point( LabelArea.Right + 7, LabelArea.Top + 5 ),
                    new Point( LabelArea.Right + 10, LabelArea.Top + LabelArea.Height - 2 ),
                    new Point( LabelArea.Right + 14, LabelArea.Bottom )
                };

                
                myGeometry.Children.Add(GeometryHelper.GetGeometryFromPoints(points));
                var body = new Rect(0, LabelArea.Height, Bounds.Width, Bounds.Height - LabelArea.Height);
                myGeometry.Children.Add(new RectangleGeometry(body));
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

        protected override Rect ComputeLabelArea(string label)
        {
            var textSize = this.ComputeFormattedTextSize(label,
                ConnectableBase.DefaultFont,
                ConnectableBase.DefaultFontSize, 5, 10);
            textSize.Width = Math.Max(textSize.Width, 80);
            var pos = new Point(5, 0);
            return new Rect(pos, textSize);
        }

        void AdjustBounds()
        {
            if (Bounds.Left != 0) // the bounds where not yet initialized
            {
                LabelArea = ComputeLabelArea(DisplayedLabel());
                var w = Math.Max(Bounds.Width, LabelArea.Width + 20);
                var h = Bounds.Height;
                Bounds = ComputeBounds(Bounds.TopLeft, new Size(w, h), LabelArea);
            }
        }

    }
}
