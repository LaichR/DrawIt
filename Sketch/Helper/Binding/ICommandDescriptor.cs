using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Input;

namespace Sketch.Helper.Binding
{
    public interface ICommandDescriptor
    {
        string Name
        {
            get;
        }

        string ToolTip
        {
            get;
        }

        Bitmap Bitmap
        {
            get;
        }

        ICommand Command
        {
            get;
        }

        List<ICommandDescriptor> SubItems
        {
            get;
        }

        bool IsSelector
        {
            get;
        }

        Action<System.Windows.Controls.Control> Initialize
        {
            get;
        }
    }
}
