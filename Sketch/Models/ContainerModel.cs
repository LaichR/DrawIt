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
            UpdateGeometry();
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
                //var boundingRect = new RectangleGeometry(
                //    new System.Windows.Rect(Bounds.TopLeft, Bounds.BottomRight), Bounds.Height / 4, Bounds.Height / 4);
                var boundingRect = new RectangleGeometry(Bounds);
                //boundingRect.Transform = new RotateTransform(90);
                return boundingRect;
            }
        }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var g = Geometry as GeometryGroup;
            var leftTop = LabelArea.TopLeft;
            g.Children.Add(new LineGeometry(new System.Windows.Point(leftTop.X, leftTop.Y + LabelArea.Height),
                    new System.Windows.Point(leftTop.X + LabelArea.Width, leftTop.Y + LabelArea.Height)));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
