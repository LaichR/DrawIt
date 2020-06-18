using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Runtime.Serialization;

namespace Sketch.Models
{
    [Serializable]
    public class ContainerModel: ConnectableBase
    {
        
        
        ObservableCollection<ModelBase> _children = new ObservableCollection<ModelBase>();
        

        public ContainerModel(Point location, Size size )
            //:base(new Guid())
        {
            Bounds = new Rect(location, size);
            LabelArea = new Rect(location, new Size(size.Width, 20));
            IsSelected = true;
            Name = "hello world";
        }

        protected ContainerModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
        }

        public ObservableCollection<ModelBase> Children
        {
            get { return _children; }
            set { SetProperty<ObservableCollection<ModelBase>>(ref _children, value); }
        }

        

        public override RectangleGeometry Outline
        {
            get
            {
                var boundingRect = new RectangleGeometry(
                    new System.Windows.Rect(Bounds.TopLeft, Bounds.BottomRight), Bounds.Height / 4, Bounds.Height / 4);
                //boundingRect.Transform = new RotateTransform(90);
                return boundingRect;
            }
        }

        public override System.Windows.Media.Geometry Geometry
        {
            get
            {
                var myGeometry = new GeometryGroup();

                //var screenLocation = this.PointToScreen(location);


                myGeometry.Children.Add(Outline);

                var leftTop = Bounds.TopLeft;

                myGeometry.Children.Add(new LineGeometry(new System.Windows.Point(leftTop.X, leftTop.Y + 15),
                    new System.Windows.Point(leftTop.X + Bounds.Width, leftTop.Y + 15)));


                FormattedText t = new FormattedText(Name, System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Blue);
                myGeometry.Children.Add(t.BuildGeometry(new System.Windows.Point(leftTop.X + 5, leftTop.Y)));
                //myGeometry.Transform = new RotateTransform(90);
                return myGeometry;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
