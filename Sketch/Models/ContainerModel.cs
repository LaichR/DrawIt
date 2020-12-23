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
using Sketch.Helper;
using System.IO;

namespace Sketch.Models
{
    [Serializable]
    public class ContainerModel: ConnectableBase, ISketchItemContainer
    {
        [PersistentField((int)ModelVersion.V_0_1,"Children")]
        byte[] _childDataBuffer;
        ObservableCollection<ISketchItemModel> _children = new ObservableCollection<ISketchItemModel>();
        
        public ContainerModel(Point location, ISketchItemContainer container,Size size )
            :base(location, container, size, "new container", ConnectableBase.DefaultColor)
        {
            _children.CollectionChanged += SketchItemsChanged;
        }



        protected ContainerModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {}

        public ObservableCollection<ISketchItemModel> SketchItems
        {
            get { return _children; }
            set { SetProperty<ObservableCollection<ISketchItemModel>>(ref _children, value); }
        }


        protected override void PrepareFieldBackup()
        {
            base.PrepareFieldBackup();
            using (var stream = new System.IO.MemoryStream())
            {
                SketchItemDisplayHelper.TakeSnapshot(stream, this);
                _childDataBuffer = stream.ToArray();
            }
        }

        protected override void FieldDataRestored()
        {
            base.FieldDataRestored();
            using (var stream = new System.IO.MemoryStream(_childDataBuffer))
            {
                SketchItemDisplayHelper.RestoreSnapshot(stream, this);
            }
        }

        protected virtual void SketchItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        { }

    }
}
