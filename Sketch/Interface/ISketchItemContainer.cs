using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;


namespace Sketch.Interface
{
    /// <summary>
    /// I SketchItem container is a SketchItemNode that provides a observable collection of SketchItems
    /// 
    /// </summary>
    public interface ISketchItemContainer : ISketchItemNode
    {
        ObservableCollection<ISketchItemModel> SketchItems
        {
            get;
        }

    }
}
