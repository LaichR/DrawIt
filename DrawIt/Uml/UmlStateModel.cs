using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch.Models;
using System.Runtime.Serialization;
using Sketch.Interface;

namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlTransitionModel))]
    public class UmlStateModel : ContainerModel
    {
        new static readonly double DefaultWidth = 150;
        new static readonly double DefaultHeight = 60;

        static readonly double DefaultRoundingEdgeRadius = DefaultHeight / 3; 

        public UmlStateModel(Point p, ISketchItemContainer container)
            : base(p, container, new Size( DefaultWidth, DefaultHeight)) 
        {
            
            
            IsSelected = true;
            CanChangeSize = true;
            Label = "new state";

        }

        protected UmlStateModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {

            var myGeometry = Geometry as GeometryGroup;
            myGeometry.Children.Clear();


            var body = new Rect(0, 0, Bounds.Width, Bounds.Height);
            
            myGeometry.Children.Add(new RectangleGeometry(body, DefaultRoundingEdgeRadius, DefaultRoundingEdgeRadius));
            myGeometry.Transform = Rotation;
            

        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get
            {
                var outline = new System.Windows.Media.RectangleGeometry(Bounds, DefaultRoundingEdgeRadius, DefaultRoundingEdgeRadius)
                { Transform = Rotation };
                return outline;
            }
        }

        protected override Rect ComputeLabelArea(string label)
        {
            return Rect.Empty;
        }

    }
}
