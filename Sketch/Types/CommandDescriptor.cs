using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UI.Utilities.Interfaces;

namespace Sketch.Types
{
    [Serializable]
    public class CommandDescriptor : BindableModel, ICommandDescriptor
    {
        string _name;
        string _toolTip;
        System.Drawing.Bitmap _bitmap;
        System.Windows.Input.ICommand _cmd;
        System.Windows.Media.Brush _background;
        readonly List<ICommandDescriptor> _subItems = null;

        public string Name
        {
            get { return _name; }
            set { SetProperty<string>(ref _name, value); }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { SetProperty<string>(ref _toolTip, value); }
        }

        public System.Drawing.Bitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
            set { SetProperty<System.Drawing.Bitmap>(ref _bitmap, value); }
        }

        public System.Windows.Input.ICommand Command
        {
            get
            {
                return _cmd;
            }
            set
            {
                SetProperty<System.Windows.Input.ICommand>(ref _cmd, value);
            }
        }

        public System.Windows.Media.Brush Background
        {
            get { return _background; }
            set { SetProperty<System.Windows.Media.Brush>(ref _background, value); }
        }

        public List<ICommandDescriptor> SubItems
        {
            get { return _subItems; }
        }

        public bool IsSelector
        {
            get;
            set;
        }

        public Action<Control> Initialize
        {
            get;
            set;
        }
    }
}
