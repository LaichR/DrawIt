using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Types
{
    
    /// <summary>
    /// This defines where the endpoint of the connector is docked on the shape
    /// </summary>
    public enum ConnectorDocking
    {
        Undefined, // the connector is new
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,
        Self = 0x10
    }
}
