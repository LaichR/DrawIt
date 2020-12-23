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

    public delegate IBoundedSketchItemModel CreateBoundedSketchItemDelegate(Point p, ISketchItemContainer container);

    /// <summary>
    /// This interface combines the ISketchItemModel with the IConnectable. The result ist therefore a 
    /// SketchItem that is connectable and has a Bound,
    /// </summary>
    public interface IBoundedSketchItemModel: ISketchItemModel, IConnectable
    {


    }
}
