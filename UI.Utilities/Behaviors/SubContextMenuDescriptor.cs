using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Bluebottle.Base.Interfaces;

namespace Bluebottle.Base.Behaviors
{
    [Serializable]
    public class SubContextMenuDescriptor : ICommandDescriptor
    {
        public string Name
        {
            get;
            set;
        }

        public string ToolTip
        {
            get;
            set;
        }

        public System.Drawing.Bitmap Bitmap
        {
            get;
            set;
        }

        public System.Windows.Input.ICommand Command
        {
            get { return null; }
        }

        public List<ICommandDescriptor> SubItems
        {
            get;
            set;
        }

        public bool IsSelector => throw new NotImplementedException();

        public Action<Control> Initialize => throw new NotImplementedException();
    }
}
