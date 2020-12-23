using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Interface
{
    /// <summary>
    /// For now, two connector types are supported. A StrightLine connecter and the AutoRouting 
    /// connector. The StrightLine connector is a simple stright line between start and end point.
    /// 
    /// An AutoRouting connector is a connector consisting of several horizontal or vertical line segments.
    /// Some heuristics are used to guess an appropriate location of start and end point
    /// within the border of the connected shapes.
    /// 
    /// </summary>
    public enum ConnectionType
    {
        StrightLine,
        AutoRouting,
    };
}
