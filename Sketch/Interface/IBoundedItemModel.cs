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

    public interface IBoundedItemModel: ISketchItemModel, IBoundsProvider
    {

        /// <summary>
        /// Some Sketch elements may require, that incoming connections are always docked to the left side
        /// and outgoing connection to the right side
        /// Other shapes may have no restrictions. This method is used to query this information
        /// </summary>
        /// <param name="incoming">indicates if this information is related
        /// to incoming or outgoing connections
        /// </param>
        /// <returns>Bitmap containing the queried constraints</returns>
        ConnectorDocking AllowableDockings(bool incoming);


        event EventHandler<OutlineChangedEventArgs> ShapeChanged;

    }
}
