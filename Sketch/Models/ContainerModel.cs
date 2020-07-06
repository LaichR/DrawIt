using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Runtime.Serialization;
using Sketch.Interface;
using Sketch.Utilities;
using System.IO;

namespace Sketch.Models
{
    [Serializable]
    public class ContainerModel: ConnectableBase, ISketchItemContainer
    {
        
        ObservableCollection<ISketchItemModel> _children = new ObservableCollection<ISketchItemModel>();
        
        public ContainerModel(Point location, Size size )
            :base(location, size, "new container", ConnectableBase.DefaultColor)
        {
            LabelArea = new Rect(location, new Size(size.Width, 20));
        }

        protected ContainerModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {}

        public ObservableCollection<ISketchItemModel> SketchItems
        {
            get { return _children; }
            set { SetProperty<ObservableCollection<ISketchItemModel>>(ref _children, value); }
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
            List<ModelBase> children = new List<ModelBase>(SketchItems.OfType<ModelBase>());
            using (var stream = new System.IO.MemoryStream())
            {
                SketchItemDisplayHelper.TakeSnapshot(stream, SketchItems);
                var buffer = stream.ToArray();
                info.AddValue("Children", buffer, typeof(byte[]));
            }
        }

        protected override void RestoreData(SerializationInfo info, StreamingContext context)
        {
            base.RestoreData(info, context);
            try
            {
                var buffer = (byte[])info.GetValue("Children", typeof(byte[]));
                using (var stream = new System.IO.MemoryStream(buffer))
                {
                    var children = new List<ISketchItemModel>();
                    SketchItemDisplayHelper.RestoreSnapshot(stream, children);
                    List<ISketchItemModel> sketchItemList = new List<ISketchItemModel>(children);
                    SketchItemDisplayHelper.RestoreChildren(SketchItems, children);
                }
            }
            catch { }
        }

    }
}
