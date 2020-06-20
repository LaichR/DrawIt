using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using Sketch.Types;

namespace Sketch.Interface
{
    public interface ISketchItemContainer
    {

        EditMode EditMode
        {
            get;
            set;
        }

        ObservableCollection<ISketchItemModel> SketchItems
        {
            get;
        }
    }
}
