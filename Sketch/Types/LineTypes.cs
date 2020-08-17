using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Interface;

namespace Sketch.Types
{
    public enum LineType
    {
        Undefined = 0,
        SelfTop = ConnectorDocking.Self << 8 |ConnectorDocking.Top,
        SelfRight = ConnectorDocking.Self << 8 | ConnectorDocking.Right,
        SelfBottom = ConnectorDocking.Self << 8 | ConnectorDocking.Bottom,
        SelfLeft = ConnectorDocking.Self << 8 | ConnectorDocking.Left,

        TopTop  = (int)ConnectorDocking.Top << 8|ConnectorDocking.Top,
        TopLeft = (int)ConnectorDocking.Top << 8 | ConnectorDocking.Left,
        TopRight = (int)ConnectorDocking.Top << 8 | ConnectorDocking.Right,
        TopBottom = (int)ConnectorDocking.Top << 8 | ConnectorDocking.Bottom,

        RightRight = (int)ConnectorDocking.Right << 8 | ConnectorDocking.Right,
        RightLeft = (int)ConnectorDocking.Right << 8 | ConnectorDocking.Left,
        RightTop = (int)ConnectorDocking.Right << 8 | ConnectorDocking.Top,
        RightBottom = (int)ConnectorDocking.Right << 8 | ConnectorDocking.Bottom,

        LeftLeft = (int)ConnectorDocking.Left << 8 | ConnectorDocking.Left,
        LeftRight = (int)ConnectorDocking.Left << 8 | ConnectorDocking.Right,
        LeftBottom = (int)ConnectorDocking.Left << 8 | ConnectorDocking.Bottom,
        LeftTop = (int)ConnectorDocking.Left << 8 | ConnectorDocking.Top,
        
        BottomBottom = (int)ConnectorDocking.Bottom << 8 | ConnectorDocking.Bottom,
        BottomTop = (int)ConnectorDocking.Bottom << 8 | ConnectorDocking.Top,
        BottomLeft = (int)ConnectorDocking.Bottom << 8 | ConnectorDocking.Left,
        BottomRight = (int)ConnectorDocking.Bottom << 8 | ConnectorDocking.Right,

    }
}
