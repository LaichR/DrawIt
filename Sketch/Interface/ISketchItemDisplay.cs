using Sketch.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sketch.Interface
{
    public interface ISketchItemDisplay: ISketchItemContainer
    {
        Canvas Canvas
        {
            get;
        }

        ISketchItemUI SelectedItem
        {
            get;
        }

        IList<ISketchItemUI> MarkedItems
        {
            get;
        }

        void ClearSelection();

        void SetSketchItemEnable(bool set);

        void ShowIntersections();

        void TakeSnapshot();

        void DropSnapshot();
        
        void BeginEdit(IEditOperation handler);

        void EndEdit();
    }
}
