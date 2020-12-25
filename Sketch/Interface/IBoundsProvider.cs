using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sketch.Interface
{
    /// <summary>
    /// This interface allows to implment algorithms on anything that a bound. Rectabgles can be wrapped easily as well.
    /// </summary>
    public interface IBoundsProvider
    {
        Rect Bounds
        {
            get;
        }
    }
}
