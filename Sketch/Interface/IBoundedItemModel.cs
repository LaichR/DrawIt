using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace Sketch.Interface
{

    public delegate IBoundedItemModel CreateBoundedSketchItemDelegate(Point p);

    public interface IBoundedItemModel: ISketchItemModel
    {

        string Name
        {
            get;
        }

        Rect Bounds
        {
            get;
        }

        event EventHandler<OutlineChangedEventArgs> ShapeChanged;

    }
}
