using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Interface
{
    
    /// <summary>
    /// This defines where the start and end points of the connector are docked on the border of the shape
    /// 
    /// </summary>
    [Flags]
    public enum ConnectorDocking
    {
        Undefined = 0, // the connector is new and not yet attached to a sketch item
        Top =     0x1,
        Right = 0x2,
        Bottom = 0x4,
        Left = 0x8,
        Self = 0x10,
        //DockingIsLocked = 0x100
    }
}
