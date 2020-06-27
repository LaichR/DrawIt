using Sketch.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Sketch.Types;

namespace Sketch.Interface
{
    public interface ISketchItemDisplay: ISketchItemContainer, IDisposable
    {
        Canvas Canvas
        {
            get;
        }

        EditMode EditMode
        {
            get;
            set;
        }

        ISketchItemUI SelectedItem
        {
            get;
        }

        IList<ISketchItemUI> MarkedItems
        {
            get;
        }

        IEnumerable<ISketchItemUI> Items
        {
            get;
        }

        event EventHandler SelectedItemChanged;

        void HandleAddConnector(object sender, EventArgs e);

        void HandleKeyDown(KeyEventArgs keyEvent);

        void ClearSelection();

        void SetSketchItemEnable(bool set);

        void ShowIntersections();

        void TakeSnapshot();

        void DropSnapshot();
        
        void BeginEdit(IEditOperation handler);

        void EndEdit();
    }
}
