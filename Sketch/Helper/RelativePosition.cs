using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Helper
{
    /// <summary>
    /// This enum describes the relative position between two connectable gadgets; the names refer always to the relative position
    /// of the "To" gadget looking from the "From" gadget 
    /// </summary>
    /// 
    [Flags]
    public enum RelativePosition
    {
        Undefined = 0, // if we have a self connection and the From is the same as the To or if the two rectangle intersect!
        N = 1,
        NE = 3,
        E = 2,
        SE = 6,
        S = 4,
        SW = 12,
        W = 8,
        NW = 9,
    }
}
