using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using Sketch.Interface;

namespace Sketch.Models
{
    internal class ConnectableBaseFactory: IBoundedItemFactory
    {
        CreateBoundedSketchItemDelegate _createConnectorEnd;
        
        public ConnectableBaseFactory()
        {
            LastCalled = DateTime.Now;
        }

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

        public DateTime LastCalled
        {
            get;
            private set;
        }

        public CreateBoundedSketchItemDelegate CreateConnectableItem
        {
            get
            {
                return _createConnectorEnd;
            }
            set
            {
                _createConnectorEnd = (point, container) => { var end = value(point, container); 
                    LastCalled = DateTime.Now; return end; };
            }
        }

    }
}
