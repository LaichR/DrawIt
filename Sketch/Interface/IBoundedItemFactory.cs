using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sketch.Interface
{
    /// <summary>
    /// This interface describes everything that is needed define an Command on the UI to create a new
    /// connectable sketch item.
    /// </summary>
    public interface IBoundedItemFactory
    {
        DateTime LastCalled
        {
            get;
        }

        string Name
        {
            get;
        }

        string ToolTip
        {
            get;
        }


        System.Drawing.Bitmap Bitmap
        {
            get;

        }

        CreateBoundedSketchItemDelegate CreateConnectableItem
        {
            get;
        }
    }

}
